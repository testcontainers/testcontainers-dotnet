namespace DotNet.Testcontainers.Tests.Fixtures
{
    using System;
    using System.Data.Common;
  using System.IO;
  using System.Threading.Tasks;
    using DotNet.Testcontainers.Builders;
    using DotNet.Testcontainers.Configurations;
    using DotNet.Testcontainers.Containers;
    using JetBrains.Annotations;
    using Npgsql;

    public static class CosmosDbFixture
    {
        [UsedImplicitly]
        public class CosmosDbDefaultFixture : DatabaseFixture<CosmosDbTestcontainer, DbConnection>
        {
            public CosmosDbTestcontainerConfiguration Configuration { get; set; }

            public CosmosDbDefaultFixture() 
                : this(new CosmosDbTestcontainerConfiguration())
            {
            }
            
            protected CosmosDbDefaultFixture(CosmosDbTestcontainerConfiguration configuration) 
            {
                this.Configuration = configuration;
                this.Container = new TestcontainersBuilder<CosmosDbTestcontainer>()
                    .WithHostname("localhost")
                    .WithImage(configuration.Image)
                    .WithPortBinding(configuration.DefaultPort)
                    .WithExposedPort(configuration.DefaultPort)
                    .WithWaitStrategy(configuration.WaitStrategy)
                    .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "1")
                    .Build();
            }

            public override Task InitializeAsync()
            {
                return this.Container.StartAsync();
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
