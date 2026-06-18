using OLI_it.Web.Tests.Fixtures;

namespace OLI_it.Web.Tests;

/// <summary>
/// Serializes matchmaking integration tests so long-running SP executions
/// do not overlap and contend on the same LocalDB test database.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class MatchmakingCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = "Matchmaking integration tests";
}
