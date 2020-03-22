namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Configurations.Databases;
  using DotNet.Testcontainers.Containers.Modules.Databases;
  using Xunit;

  public class OracleFixture : IAsyncLifetime
  {
    public OracleTestcontainer OracleTestcontainer { get; }

    private string Username { get; } = "system";

    private string Password { get; } = "oracle";

    private string DatabaseName { get; } = "localhost";

    private int Port { get; } = 1521;

    public OracleFixture()
    {
      this.OracleTestcontainer = new TestcontainersBuilder<OracleTestcontainer>()
        .WithDatabase(new OracleTestcontainerConfiguration
        {
          Username = this.Username,
          Password = this.Password,
          Database = this.DatabaseName,
          Port = this.Port
        })
        .Build();
    }

    public async Task InitializeAsync()
    {
      await this.OracleTestcontainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
      await this.OracleTestcontainer.DisposeAsync();
    }
  }
}
