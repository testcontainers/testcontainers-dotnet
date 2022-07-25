namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  [PublicAPI]
    public class CosmosDbTestcontainerConfiguration : TestcontainerDatabaseConfiguration
    {
        private const string CosmosDbImage = "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator";

        private const int CosmosDbPort = 8081;

        public CosmosDbTestcontainerConfiguration()
            : this(CosmosDbImage)
        {

        }

        public CosmosDbTestcontainerConfiguration(string image)
            : base(image, CosmosDbPort)
        {
        }

        public override string Database
        {
            get => this.Environments["AZURE_COSMOS_EMULATOR_DATABASE"];
            set => this.Environments["AZURE_COSMOS_EMULATOR_DATABASE"] = value;
        }

        public string PartitionCount
        {
            get => this.Environments["AZURE_COSMOS_EMULATOR_PARTITION_COUNT"];
            set => this.Environments["AZURE_COSMOS_EMULATOR_PARTITION_COUNT"] = value;
        }

        public string EnableDataPersistence
        {
            get => this.Environments["AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE"];
            set => this.Environments["AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE"] = value;
        }

        public string IPAddressOverride
        {
            get => this.Environments["AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE"];
            set => this.Environments["AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE"] = value;
        }

        public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
            .UntilMessageIsLogged(this.OutputConsumer.Stdout, "Started");
    }
}