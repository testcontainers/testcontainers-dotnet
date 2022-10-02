namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Globalization;
  using System.IO;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  [PublicAPI]
  public sealed class CosmosDbTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public const string DefaultCosmosDbApiImage =
      "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator";

    private const int DefaultCosmosPort = 8081;

    public CosmosDbTestcontainerConfiguration()
      : this(DefaultCosmosDbApiImage)
    {
    }

    public CosmosDbTestcontainerConfiguration(string image)
      : base(image, DefaultCosmosPort)
    {
      this.PartitionCount = 2;
      this.IpAddressOverride = "127.0.0.1";
      this.OutputConsumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());
      this.Database = "default";
    }

    public override IOutputConsumer OutputConsumer { get; }

    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer().UntilMessageIsLogged(this.OutputConsumer.Stdout, "Started|Shutting");

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
      get => int.TryParse(this.Environments["AZURE_COSMOS_EMULATOR_PARTITION_COUNT"], NumberStyles.Integer, CultureInfo.InvariantCulture, out var partitionCount) ? partitionCount : 1;
      set => this.Environments["AZURE_COSMOS_EMULATOR_PARTITION_COUNT"] = value.ToString(CultureInfo.InvariantCulture);
    }

    public bool EnableDataPersistence
    {
      get => bool.TryParse(this.Environments["AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE"], out var enableDataPersistence) && enableDataPersistence;
      set => this.Environments["AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE"] = value.ToString().ToLowerInvariant();
    }

    public string IpAddressOverride
    {
      get => this.Environments["AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE"];
      set => this.Environments["AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE"] = value;
    }
  }
}
