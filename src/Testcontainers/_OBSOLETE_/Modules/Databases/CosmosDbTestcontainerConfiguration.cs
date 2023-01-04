namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Globalization;
  using System.IO;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public sealed class CosmosDbTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public const string CosmosDbImage = "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest";

    private const int CosmosDbPort = 8081;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbTestcontainerConfiguration" /> class.
    /// </summary>
    public CosmosDbTestcontainerConfiguration()
      : this(CosmosDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public CosmosDbTestcontainerConfiguration(string image)
      : base(image, CosmosDbPort)
    {
      this.Database = "default";
      this.IpAddressOverride = "127.0.0.1";
      this.PartitionCount = 2;
      this.OutputConsumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());
      this.WaitStrategy = Wait.ForUnixContainer().UntilMessageIsLogged(this.OutputConsumer.Stdout, "Started|Shutting");
    }

    /// <inheritdoc />
    public override string Database
    {
      get => this.Environments["AZURE_COSMOS_EMULATOR_DATABASE"];
      set => this.Environments["AZURE_COSMOS_EMULATOR_DATABASE"] = value;
    }

    /// <inheritdoc />
    public override string Password
    {
      // Default Cosmos DB Emulator authentication key: https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21#authenticate-requests.
      get => "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
      set => throw new NotImplementedException();
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

    public int PartitionCount
    {
      get => int.TryParse(this.Environments["AZURE_COSMOS_EMULATOR_PARTITION_COUNT"], NumberStyles.Integer, CultureInfo.InvariantCulture, out var partitionCount) ? partitionCount : 1;
      set => this.Environments["AZURE_COSMOS_EMULATOR_PARTITION_COUNT"] = value.ToString(CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override IOutputConsumer OutputConsumer { get; }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy { get; }
  }
}
