using Bugget.IntegrationTests.Fixtures;
using Xunit;

namespace Bugget.IntegrationTests;

[CollectionDefinition("PostgresCollection")]
public class PostgresCollection : ICollectionFixture<PostgresContainerFixture> { }

