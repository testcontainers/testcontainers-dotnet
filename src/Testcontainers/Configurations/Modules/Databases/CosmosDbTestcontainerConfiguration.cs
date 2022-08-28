namespace DotNet.Testcontainers.Configurations
{
    using System;
    using DotNet.Testcontainers.Builders;
    using JetBrains.Annotations;

    [PublicAPI]
    public class CosmosDbTestcontainerConfiguration : TestcontainerDatabaseConfiguration
    {
        // TODO: WithMongoAPI extension?
        public const string DefaultCosmosDbSqlApiImage = 
            "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator";

        public const string DefaultCosmosDbMongoDbApiImage = 
            "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:mongo";

        private const int DefaultSqlApiPort = 8081;
        
        private const int DefaultMongoDbApiPort = 10250;

        public CosmosDbTestcontainerConfiguration()
            : this(DefaultCosmosDbSqlApiImage) 
        {
        }

        public CosmosDbTestcontainerConfiguration(string image) 
            : base(image, DefaultSqlApiPort) 
        {
            this.Environments.Add("AZURE_COSMOS_EMULATOR_MONGO_DB_ENDPOINT", "");
            this.Environments.Add("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "1");
        }

        public int MongoDbApiPort { get; set; }

        public int MongoDbApiContainerPort { get; set; }

        public int SqlApiPort { get; set; }

        public int SqlApiContainerPort { get; set; }

        public override IWaitForContainerOS WaitStrategy 
        {
            get
            {
                var waitStrategy = Wait.ForUnixContainer();
                // waitStrategy = string.IsNullOrWhiteSpace(this.MongoDbEndpoint) 
                //     ? waitStrategy.UntilPortIsAvailable(SqlApiContainerPort)
                //     : waitStrategy.UntilPortIsAvailable(MongoDbApiContainerPort);
                waitStrategy = waitStrategy.UntilMessageIsLogged(this.OutputConsumer.Stdout, "Started");

                return waitStrategy;
            }
        }

        public override string Password
        {
            // Default key for the emulator
            // See: https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21#authenticate-requests
            get => "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            set => throw new NotImplementedException();
        }

        public override string Database
        {
            get => this.Environments["AZURE_COSMOS_EMULATOR_DATABASE"];
            set => this.Environments["AZURE_COSMOS_EMULATOR_DATABASE"] = value;
        }

        public int PartitionCount
        {
            get => int.Parse(this.Environments["AZURE_COSMOS_EMULATOR_PARTITION_COUNT"]);
            set => this.Environments["AZURE_COSMOS_EMULATOR_PARTITION_COUNT"] = value.ToString();
        }

        public bool EnableDataPersistence
        {
            get => this.Environments["AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE"] == "true";
            set => this.Environments["AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE"] = value.ToString().ToLower();
        }

        public string IPAddressOverride
        {
            get => this.Environments["AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE"];
            set => this.Environments["AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE"] = value;
        } 

        public string MongoDbEndpoint
        {
            get => this.Environments["AZURE_COSMOS_EMULATOR_MONGO_DB_ENDPOINT"];
            set => this.Environments["AZURE_COSMOS_EMULATOR_MONGO_DB_ENDPOINT"] = value;
        } 
    }
}
