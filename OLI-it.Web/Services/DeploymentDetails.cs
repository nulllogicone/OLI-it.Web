using System.Reflection;
using Azure.Storage.Blobs;
using Microsoft.Data.SqlClient;

namespace OLI_it.Web.Services;

public sealed class DeploymentDetails(IConfiguration configuration, string databaseConnectionName)
{
    private const string Unknown = "Unavailable";

    public string Machine { get; } = Environment.MachineName;
    public string DatabaseConnectionName { get; } = databaseConnectionName;
    public string Database { get; } = DescribeDatabase(configuration.GetConnectionString(databaseConnectionName));
    public string Storage { get; } = DescribeStorage(configuration);
    public string Branch { get; } = ReadBuildMetadata("DeploymentBranch");
    public string Commit { get; } = ReadBuildMetadata("DeploymentCommit");
    public string CommitSubject { get; } = ReadBuildMetadata("DeploymentCommitSubject");
    public string BuiltAt { get; } = ReadBuildMetadata("DeploymentBuildUtc");

    private static string DescribeDatabase(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return "Unconfigured";

        if (connectionString.StartsWith("@Microsoft.KeyVault(", StringComparison.OrdinalIgnoreCase))
            return "Unresolved Key Vault reference";

        try
        {
            var settings = new SqlConnectionStringBuilder(connectionString);
            return string.IsNullOrWhiteSpace(settings.DataSource)
                ? Unknown
                : $"{settings.DataSource} / {DefaultIfBlank(settings.InitialCatalog, "default database")}";
        }
        catch (ArgumentException)
        {
            return Unknown;
        }
    }

    private static string DescribeStorage(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OliItStorageConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
            return "Unconfigured";

        try
        {
            var storage = new BlobServiceClient(connectionString);
            var container = configuration["AzureStorage:ContainerName"] ?? "oliupload";
            return $"{DefaultIfBlank(storage.AccountName, Unknown)} / {container}";
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException)
        {
            return Unknown;
        }
    }

    private static string ReadBuildMetadata(string key)
    {
        var value = typeof(DeploymentDetails).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attribute => attribute.Key == key)?.Value;
        return DefaultIfBlank(value, Unknown);
    }

    private static string DefaultIfBlank(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value;
}
