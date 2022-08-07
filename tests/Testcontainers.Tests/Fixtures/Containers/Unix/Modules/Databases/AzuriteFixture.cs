namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using Azure.Data.Tables;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations.Modules.Databases;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class AzuriteFixture : IAsyncLifetime, IDisposable
  {
    private readonly AzuriteTestcontainerConfiguration configuration = new();

    public AzuriteFixture()
    {
      this.Container = new TestcontainersBuilder<AzuriteTestcontainer>()
        .WithAzurite(this.configuration)
        .Build();
    }

    public AzuriteTestcontainer Container { get; }

    public TableServiceClient Connection { get; private set; }

    public async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      this.Connection = new TableServiceClient(this.Container.ConnectionString);
    }

    public async Task DisposeAsync()
    {
      await this.Container.DisposeAsync()
        .ConfigureAwait(false);
    }

    public void Dispose()
    {
      this.configuration.Dispose();
    }
  }
}
