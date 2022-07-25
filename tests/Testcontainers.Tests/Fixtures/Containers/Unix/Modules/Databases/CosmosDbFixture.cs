namespace DotNet.Testcontainers.Tests.Fixtures 
{
    using System.Data.Common;
    using System.Threading.Tasks;
    using DotNet.Testcontainers.Builders;
    using DotNet.Testcontainers.Configurations;
    using DotNet.Testcontainers.Containers;
    using Npgsql;

    public sealed class CosmosDbFixture : DatabaseFixture<CosmosDbTestcontainer, DbConnection>
    {
        private readonly TestcontainerDatabaseConfiguration configuration = 
            new CosmosDbTestcontainerConfiguration{ Database = "testdb" };

        public CosmosDbFixture()
        {
            this.Container = new TestcontainersBuilder<CosmosDbTestcontainer>()
                .WithDatabase(this.configuration)
                .Build();
        }

        public override async Task InitializeAsync()
        {
            await this.Container.StartAsync()
                .ConfigureAwait(false);

            // var dbResponse = await this.Container.CreateDatabaseAsync()
            //     .ConfigureAwait(false);

            // if (dbResponse.StatusCode != System.Net.HttpStatusCode.Created)
            // {
            //     throw new System.Exception("Failed to create database");
            // }
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