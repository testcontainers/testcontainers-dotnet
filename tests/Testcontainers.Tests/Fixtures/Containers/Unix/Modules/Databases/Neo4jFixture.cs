namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Neo4j.Driver;

  [UsedImplicitly]
  public sealed class Neo4jFixture : DatabaseFixture<Neo4jTestcontainer, IDriver>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new Neo4jTestcontainerConfiguration();

    public Neo4jFixture()
    {
      this.Container = new TestcontainersBuilder<Neo4jTestcontainer>()
        .WithDatabase(this.configuration)
        .Build();
    }

    public override async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      this.Connection = GraphDatabase.Driver(this.Container.ConnectionString, AuthTokens.Basic(this.Container.Username, this.Container.Password));
    }

    public override async Task DisposeAsync()
    {
      this.Connection.Dispose();

      await this.Container.DisposeAsync()
        .ConfigureAwait(false);
    }

    public override void Dispose()
    {
      this.configuration.Dispose();
    }
  }
}
