namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using Containers.Configurations.Databases;
  using Containers.Modules.Databases;
  using DotNet.Testcontainers.Containers.Builders;
  using Xunit;

  public class OracleDbFixture : IAsyncLifetime
  {
    public OracleTestContainer OracleTestContainer { get; }
    public string Username { get; } = "system";
    public string Password { get; } = "oracle";
    public string DatabaseName { get; } = "localhost";
    public int Port { get; } = 1521;

    public OracleDbFixture()
    {
      this.OracleTestContainer = new TestcontainersBuilder<OracleTestContainer>()
        .WithDatabase(new OracleDbTestontainerConfiguration
        {
          Username = this.Username, Password = this.Password, Database = this.DatabaseName, Port = this.Port
        })
        .Build();
    }

    public async Task InitializeAsync()
    {
      await this.OracleTestContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
      await this.OracleTestContainer.StopAsync();
      await this.OracleTestContainer.DisposeAsync();
    }
  }
}

