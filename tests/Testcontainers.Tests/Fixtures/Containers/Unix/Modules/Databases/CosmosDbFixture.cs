namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Collections.Generic;
  using System.Data.Common;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  public static class CosmosDbFixture
  {
    [UsedImplicitly]
    public class CosmosDbDefaultFixture : DatabaseFixture<CosmosDbTestcontainer, DbConnection>
    {
      public CosmosDbDefaultFixture()
        : this(new CosmosDbTestcontainerConfiguration())
      {
      }

      protected CosmosDbDefaultFixture(CosmosDbTestcontainerConfiguration configuration)
      {
        this.Configuration = configuration;
        this.Container = new TestcontainersBuilder<CosmosDbTestcontainer>()
          .WithImage(configuration.Image)
          .WithPortBinding(configuration.DefaultPort)
          .WithExposedPort(configuration.DefaultPort)
          .WithWaitStrategy(configuration.WaitStrategy)
          .WithOutputConsumer(configuration.OutputConsumer)
          .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "1")
          .ConfigureContainer(testcontainer =>
          {
            testcontainer.ContainerSqlApiPort = configuration.DefaultPort;
            testcontainer.Password = configuration.Password;
          })
          .Build();
      }

      public CosmosDbTestcontainerConfiguration Configuration { get; set; }


      public override Task InitializeAsync()
      {
        // wait for 5 min
        return Task.WhenAny(this.Container.StartAsync(), Task.Delay(5 * 60 * 1000));
      }

      public override async Task DisposeAsync()
      {
        if (Connection != null && Connection.State != System.Data.ConnectionState.Closed)
        {
          this.Connection.Dispose();
        }

        await this.Container.DisposeAsync()
          .ConfigureAwait(false);
      }

      public override void Dispose()
      {
        this.Configuration.Dispose();
      }
    }
  }
}
