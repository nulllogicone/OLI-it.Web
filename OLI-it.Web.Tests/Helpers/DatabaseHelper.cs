using System.Data;
using Microsoft.Data.SqlClient;

namespace OLI_it.Web.Tests.Helpers;

/// <summary>
/// Low-level helpers for provisioning and managing the LocalDB test database.
/// All methods operate on the master database connection except where noted.
/// </summary>
internal static class DatabaseHelper
{
    private const string MasterConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=master;Integrated Security=true;TrustServerCertificate=true;";

    /// <summary>
    /// Restores a SQL Server .bak file as a new database with the given name.
    /// Any existing database with that name is dropped first.
    /// </summary>
    public static async Task RestoreBackupAsync(string bakFilePath, string databaseName)
    {
        string fullBakPath = Path.GetFullPath(bakFilePath);

        if (!File.Exists(fullBakPath))
            throw new FileNotFoundException($"Backup file not found: {fullBakPath}");

        await DropDatabaseAsync(databaseName);

        // Discover the logical file names and types inside the backup
        var logicalFiles = await GetLogicalFileListAsync(fullBakPath);

        var dataFile = logicalFiles.FirstOrDefault(f => f.Type == "D")
            ?? throw new InvalidOperationException(
                $"No data file (Type=D) found in backup '{fullBakPath}'. " +
                $"Files found: {string.Join(", ", logicalFiles.Select(f => $"{f.Name}({f.Type})"))}");

        var logFile = logicalFiles.FirstOrDefault(f => f.Type == "L")
            ?? throw new InvalidOperationException(
                $"No log file (Type=L) found in backup '{fullBakPath}'. " +
                $"Files found: {string.Join(", ", logicalFiles.Select(f => $"{f.Name}({f.Type})"))}");

        // Resolve LocalDB default data directory
        string dataDir = await GetLocalDbDataDirectoryAsync();
        string mdfPath = Path.Combine(dataDir, $"{databaseName}.mdf");
        string ldfPath = Path.Combine(dataDir, $"{databaseName}_log.ldf");

        string sql = $"""
            RESTORE DATABASE [{databaseName}]
            FROM DISK = N'{fullBakPath}'
            WITH
                MOVE N'{dataFile.Name}' TO N'{mdfPath}',
                MOVE N'{logFile.Name}' TO N'{ldfPath}',
                REPLACE, RECOVERY
            """;

        await using var conn = new SqlConnection(MasterConnectionString);
        await conn.OpenAsync();

        // Pre-flight: check backup version vs LocalDB version
        await CheckBackupCompatibilityAsync(conn, fullBakPath);

        Console.WriteLine($"[DatabaseHelper] Restoring '{fullBakPath}' → [{databaseName}]");
        Console.WriteLine($"[DatabaseHelper]   data: {dataFile.Name} → {mdfPath}");
        Console.WriteLine($"[DatabaseHelper]   log : {logFile.Name} → {ldfPath}");

        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 300 };
        try
        {
            await cmd.ExecuteNonQueryAsync();
            Console.WriteLine($"[DatabaseHelper] Restore succeeded.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"RESTORE DATABASE failed for backup '{fullBakPath}' → database '{databaseName}'. " +
                $"data='{mdfPath}', log='{ldfPath}'. Error ({ex.GetType().Name}): {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Drops the given database from LocalDB if it exists.
    /// </summary>
    public static async Task DropDatabaseAsync(string databaseName)
    {
        string sql = $"""
            IF DB_ID(N'{databaseName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{databaseName}];
            END
            """;

        await using var conn = new SqlConnection(MasterConnectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 60 };
        await cmd.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Reads a .sql file and executes it against the given connection string.
    /// Returns false (and does nothing) if the file is empty or contains only the placeholder comment.
    /// </summary>
    public static async Task<bool> ApplyCandidateSpAsync(string connectionString, string sqlFilePath)
    {
        if (!File.Exists(sqlFilePath))
            return false;

        string sql = await File.ReadAllTextAsync(sqlFilePath);

        // Ignore placeholder files (only contain comments/whitespace)
        string stripped = string.Concat(sql.Split('\n')
            .Where(l => !l.TrimStart().StartsWith("--"))
            .Select(l => l.Trim()));

        if (string.IsNullOrWhiteSpace(stripped))
            return false;

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 120 };
        await cmd.ExecuteNonQueryAsync();
        return true;
    }

    /// <summary>
    /// Executes a stored procedure and returns the elapsed wall-clock time.
    /// For <c>fischen</c>, progress is shown with processed item counts and ETA.
    /// </summary>
    public static async Task<TimeSpan> ExecuteStoredProcedureAsync(string connectionString, string procedureName)
    {
        // fischen supports @CodeGuid/@AnglerGuid filtering, so run it per CodeGuid for real progress/ETA.
        if (procedureName.EndsWith("fischen", StringComparison.OrdinalIgnoreCase) ||
            procedureName.EndsWith(".fischen", StringComparison.OrdinalIgnoreCase))
        {
            return await ExecuteFischenWithProgressAsync(connectionString, procedureName);
        }

        await using var conn = new SqlConnection(connectionString);
        conn.InfoMessage += (_, e) => Console.WriteLine($"[SP] {e.Message}");

        await conn.OpenAsync();
        await using var cmd = new SqlCommand(procedureName, conn)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 3600
        };

        Console.WriteLine($"[DatabaseHelper] Executing [{procedureName}] ...");
        var sw = System.Diagnostics.Stopwatch.StartNew();

        using var cts = new CancellationTokenSource();
        var ticker = Task.Run(async () =>
        {
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), cts.Token);
                    Console.WriteLine($"[DatabaseHelper]   ... still running ({sw.Elapsed:mm\\:ss} elapsed)");
                }
            }
            catch (OperationCanceledException) { }
        });

        try
        {
            await cmd.ExecuteNonQueryAsync();
        }
        finally
        {
            cts.Cancel();
            await ticker;
        }

        sw.Stop();
        Console.WriteLine($"[DatabaseHelper] [{procedureName}] completed in {sw.Elapsed:mm\\:ss\\.ff}");
        return sw.Elapsed;
    }

    private static async Task<TimeSpan> ExecuteFischenWithProgressAsync(string connectionString, string procedureName)
    {
        await using var conn = new SqlConnection(connectionString);
        conn.InfoMessage += (_, e) => Console.WriteLine($"[SP] {e.Message}");
        await conn.OpenAsync();

        var scale = await GetMatchmakingScaleAsync(conn);
        int totalCodes = scale.CodeCount;
        int anglers = scale.AnglerCount;
        long totalPairs = (long)totalCodes * anglers;

        Console.WriteLine(
            $"[DatabaseHelper] Scale: {totalCodes} Code × {anglers} Angler = {totalPairs} pairs to evaluate " +
            $"({scale.SpiegelBefore} Spiegel rows before run)");

        if (totalCodes == 0)
        {
            Console.WriteLine("[DatabaseHelper] No Code rows found, skipping fischen.");
            return TimeSpan.Zero;
        }

        var codeGuids = await GetAllCodeGuidsAsync(conn);

        await using var cmd = new SqlCommand(procedureName, conn)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 3600
        };
        var codeParam = cmd.Parameters.Add("@CodeGuid", SqlDbType.UniqueIdentifier);
        var anglerParam = cmd.Parameters.Add("@AnglerGuid", SqlDbType.UniqueIdentifier);
        anglerParam.Value = Guid.Empty; // all Angler for each Code

        Console.WriteLine($"[DatabaseHelper] Executing [{procedureName}] in {totalCodes} Code-guided batches ...");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var lastLog = TimeSpan.Zero;

        for (int i = 0; i < codeGuids.Count; i++)
        {
            codeParam.Value = codeGuids[i];
            await cmd.ExecuteNonQueryAsync();

            // Time-based progress output (every 30s) + final line
            int processedCodes = i + 1;
            if (sw.Elapsed - lastLog < TimeSpan.FromSeconds(30) && processedCodes < totalCodes)
                continue;

            lastLog = sw.Elapsed;
            long processedPairs = (long)processedCodes * anglers;
            double pct = totalPairs > 0 ? (double)processedPairs / totalPairs * 100 : 100;
            double pairsPerSecond = sw.Elapsed.TotalSeconds > 0 ? processedPairs / sw.Elapsed.TotalSeconds : 0;
            long remainingPairs = Math.Max(0, totalPairs - processedPairs);
            TimeSpan eta = pairsPerSecond > 0
                ? TimeSpan.FromSeconds(remainingPairs / pairsPerSecond)
                : TimeSpan.Zero;

            Console.WriteLine(
                $"[DatabaseHelper]   progress: {processedCodes}/{totalCodes} Code " +
                $"({processedPairs:N0}/{totalPairs:N0} pairs, {pct:F1}%) | " +
                $"elapsed {sw.Elapsed:mm\\:ss} | eta {eta:mm\\:ss}");
        }

        sw.Stop();
        Console.WriteLine($"[DatabaseHelper] [{procedureName}] completed in {sw.Elapsed:mm\\:ss\\.ff}");
        return sw.Elapsed;
    }

    private static async Task<(int CodeCount, int AnglerCount, int SpiegelBefore)> GetMatchmakingScaleAsync(SqlConnection conn)
    {
        const string sql = """
            SELECT
                (SELECT COUNT(*) FROM [oli].[Code]) AS CodeCount,
                (SELECT COUNT(*) FROM [oli].[Angler]) AS AnglerCount,
                (SELECT COUNT(*) FROM [oli].[Spiegel]) AS SpiegelBefore
            """;

        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 30 };
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return (0, 0, 0);

        return (reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2));
    }

    private static async Task<List<Guid>> GetAllCodeGuidsAsync(SqlConnection conn)
    {
        const string sql = "SELECT [CodeGuid] FROM [oli].[Code] ORDER BY [CodeGuid]";
        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 30 };
        await using var reader = await cmd.ExecuteReaderAsync();

        var codeGuids = new List<Guid>();
        while (await reader.ReadAsync())
            codeGuids.Add(reader.GetGuid(0));

        return codeGuids;
    }

    /// <summary>
    /// Reads all rows from oli.Spiegel in the test database.
    /// </summary>
    public static async Task<IReadOnlyList<SpiegelRow>> ReadSpiegelAsync(string connectionString)
    {
        const string sql = "SELECT CodeGuid, AnglerGuid, Status FROM [oli].[Spiegel]";

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 60 };
        await using var reader = await cmd.ExecuteReaderAsync();

        var rows = new List<SpiegelRow>();
        while (await reader.ReadAsync())
        {
            rows.Add(new SpiegelRow(
                CodeGuid: reader.GetGuid(0),
                AnglerGuid: reader.GetGuid(1),
                Status: reader.IsDBNull(2) ? null : reader.GetString(2)
            ));
        }
        return rows;
    }

    private static async Task<List<LogicalFile>> GetLogicalFileListAsync(string bakPath)
    {
        string sql = $"RESTORE FILELISTONLY FROM DISK = N'{bakPath}'";
        var files = new List<LogicalFile>();

        await using var conn = new SqlConnection(MasterConnectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 120 };
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            string name = reader.GetString(reader.GetOrdinal("LogicalName"));
            string type = reader.GetString(reader.GetOrdinal("Type")).Trim();
            files.Add(new LogicalFile(name, type));
        }

        return files;
    }

    private sealed record LogicalFile(string Name, string Type);

    private static async Task CheckBackupCompatibilityAsync(SqlConnection conn, string bakPath)
    {
        // Get LocalDB version
        await using var verCmd = new SqlCommand("SELECT @@VERSION, SERVERPROPERTY('ProductMajorVersion')", conn);
        await using var verReader = await verCmd.ExecuteReaderAsync();
        string localDbVersion = "unknown";
        int localDbMajor = 0;
        if (await verReader.ReadAsync())
        {
            localDbVersion = verReader.GetString(0).Split('\n')[0].Trim();
            localDbMajor = verReader.IsDBNull(1) ? 0 : Convert.ToInt32(verReader.GetValue(1));
        }
        await verReader.CloseAsync();

        // Get backup version via RESTORE HEADERONLY
        int backupMajor = 0;
        string backupSoftware = "unknown";
        await using var hdrCmd = new SqlCommand(
            $"RESTORE HEADERONLY FROM DISK = N'{bakPath}'", conn) { CommandTimeout = 60 };
        await using var hdrReader = await hdrCmd.ExecuteReaderAsync();
        if (await hdrReader.ReadAsync())
        {
            // SoftwareVersionMajor is column index 22 in most SQL Server versions
            try
            {
                int colIdx = hdrReader.GetOrdinal("SoftwareVersionMajor");
                backupMajor = hdrReader.IsDBNull(colIdx) ? 0 : hdrReader.GetInt32(colIdx);
            }
            catch { /* column may not exist on all versions */ }
            try
            {
                int softwareCol = hdrReader.GetOrdinal("ServerName");
                backupSoftware = hdrReader.IsDBNull(softwareCol) ? "unknown" : hdrReader.GetString(softwareCol);
            }
            catch { /* ignore */ }
        }
        await hdrReader.CloseAsync();

        if (backupMajor > 0 && localDbMajor > 0 && backupMajor > localDbMajor)
        {
            throw new InvalidOperationException(
                $"SQL Server version mismatch: backup was created with SQL Server major version {backupMajor} " +
                $"(server: {backupSoftware}) but LocalDB is version {localDbMajor} ({localDbVersion}). " +
                $"Install SQL Server LocalDB {backupMajor} or higher, or use a compatible backup.");
        }
    }

    private static async Task<string> GetLocalDbDataDirectoryAsync()
    {
        const string sql = "SELECT SERVERPROPERTY('InstanceDefaultDataPath')";
        await using var conn = new SqlConnection(MasterConnectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        var result = await cmd.ExecuteScalarAsync();
        string path = result?.ToString() ?? Path.GetTempPath();
        return path.TrimEnd('\\', '/');
    }
}
