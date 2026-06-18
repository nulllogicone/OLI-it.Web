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

        await DropDatabaseAsync(databaseName);

        // Discover the logical file names inside the backup
        var logicalFiles = await GetLogicalFileNamesAsync(fullBakPath);

        string? dataLogical = logicalFiles.FirstOrDefault(f => !f.EndsWith("_log", StringComparison.OrdinalIgnoreCase));
        string? logLogical = logicalFiles.FirstOrDefault(f => f.EndsWith("_log", StringComparison.OrdinalIgnoreCase));

        if (dataLogical is null || logLogical is null)
            throw new InvalidOperationException(
                $"Could not identify data and log logical file names in '{fullBakPath}'. " +
                $"Found: {string.Join(", ", logicalFiles)}");

        // Resolve LocalDB default data directory
        string dataDir = await GetLocalDbDataDirectoryAsync();
        string mdfPath = Path.Combine(dataDir, $"{databaseName}.mdf");
        string ldfPath = Path.Combine(dataDir, $"{databaseName}_log.ldf");

        string sql = $"""
            RESTORE DATABASE [{databaseName}]
            FROM DISK = N'{fullBakPath}'
            WITH
                MOVE N'{dataLogical}' TO N'{mdfPath}',
                MOVE N'{logLogical}' TO N'{ldfPath}',
                REPLACE, STATS = 0, RECOVERY
            """;

        await using var conn = new SqlConnection(MasterConnectionString);
        await conn.OpenAsync();

        // RESTORE requires a single-user context; issue it with sufficient timeout
        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 300 };
        await cmd.ExecuteNonQueryAsync();
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
    /// Executes a stored procedure (no parameters) and returns the elapsed wall-clock time.
    /// </summary>
    public static async Task<TimeSpan> ExecuteStoredProcedureAsync(string connectionString, string procedureName)
    {
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(procedureName, conn)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 600
        };

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await cmd.ExecuteNonQueryAsync();
        sw.Stop();
        return sw.Elapsed;
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

    private static async Task<List<string>> GetLogicalFileNamesAsync(string bakPath)
    {
        string sql = $"RESTORE FILELISTONLY FROM DISK = N'{bakPath}'";
        var names = new List<string>();

        await using var conn = new SqlConnection(MasterConnectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 120 };
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            names.Add(reader.GetString(reader.GetOrdinal("LogicalName")));

        return names;
    }

    private static async Task<string> GetLocalDbDataDirectoryAsync()
    {
        const string sql = "SELECT SERVERPROPERTY('InstanceDefaultDataPath')";
        await using var conn = new SqlConnection(MasterConnectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        var result = await cmd.ExecuteScalarAsync();
        return result?.ToString()?.TrimEnd('\\') ?? Path.GetTempPath();
    }
}
