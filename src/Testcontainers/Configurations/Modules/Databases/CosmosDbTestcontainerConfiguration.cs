namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Globalization;
  using System.IO;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  [PublicAPI]
  public class CosmosDbTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public const string DefaultCosmosDbApiImage =
      "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator";

    private const int DefaultCosmosPort = 8081;

    public CosmosDbTestcontainerConfiguration()
      : this(DefaultCosmosDbApiImage)
    {
      this.OutputConsumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());
    }

    public CosmosDbTestcontainerConfiguration(string image)
      : base(image, DefaultCosmosPort)
    {
      this.Environments.Add("AZURE_COSMOS_EMULATOR_MONGO_DB_ENDPOINT", string.Empty);
      this.Environments.Add("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "1");
    }

    public override IOutputConsumer OutputConsumer { get; }

    public override IWaitForContainerOS WaitStrategy
    {
      get
      {
        var waitStrategy = Wait.ForUnixContainer();
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
      get => int.Parse(this.Environments["AZURE_COSMOS_EMULATOR_PARTITION_COUNT"], CultureInfo.InvariantCulture);
      set => this.Environments["AZURE_COSMOS_EMULATOR_PARTITION_COUNT"] = value.ToString(CultureInfo.InvariantCulture);
    }

    public bool EnableDataPersistence
    {
      get => this.Environments["AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE"] == "true";
      set => this.Environments["AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE"] = value.ToString().ToLower(CultureInfo.InvariantCulture);
    }

    public string IpAddressOverride
    {
      get => this.Environments["AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE"];
      set => this.Environments["AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE"] = value;
    }
  }
}
