using Microsoft.Extensions.Configuration;
using OLI_it.Web.Tests.Helpers;

namespace OLI_it.Web.Tests.Fixtures;

/// <summary>
/// xUnit class fixture that provisions a fresh LocalDB database from a .bak backup
/// before each test class that uses it, and drops it afterwards.
/// </summary>
public sealed class DatabaseFixture : IAsyncLifetime
{
    private readonly IConfiguration _config;

    public string ConnectionString { get; private set; } = string.Empty;
    public string FischenProcedure { get; private set; } = string.Empty;
    public string BackupFilePath { get; private set; } = string.Empty;

    /// <summary>
    /// When true the backup file was found and the database was provisioned successfully.
    /// Tests should skip when this is false.
    /// </summary>
    public bool IsAvailable { get; private set; }

    private const string DatabaseName = "OliItMatchmakingTest";

    public DatabaseFixture()
    {
        _config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Tests.json", optional: false)
            .Build();
    }

    public async Task InitializeAsync()
    {
        BackupFilePath = Path.GetFullPath(
            _config["Matchmaking:BackupFilePath"] ?? "TestData/oli-it-backup.bak");

        FischenProcedure = _config["Matchmaking:FischenProcedure"]
            ?? throw new InvalidOperationException(
                "Matchmaking:FischenProcedure is not set in appsettings.Tests.json.");

        var baseCs = _config.GetConnectionString("OliItTestDb")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:OliItTestDb is not set in appsettings.Tests.json.");

        ConnectionString = baseCs;

        if (!File.Exists(BackupFilePath))
        {
            IsAvailable = false;
            return;
        }

        await DatabaseHelper.RestoreBackupAsync(BackupFilePath, DatabaseName);
        IsAvailable = true;
    }

    public async Task DisposeAsync()
    {
        if (IsAvailable)
            await DatabaseHelper.DropDatabaseAsync(DatabaseName);
    }
}
