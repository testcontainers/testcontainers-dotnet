using System.Net;

namespace Testcontainers.CosmosDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class CosmosDbBuilder : ContainerBuilder<CosmosDbBuilder, CosmosDbContainer, CosmosDbConfiguration>
{
    public const string CosmosDbEmulatorImage = "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest";
    
    public const ushort CosmosDbPort = 8081;
    
    public const string DefaultAccountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    
    public const int DefaultPartitionCount = 2;
    
    public const string DefaultIpAddressOverride = "127.0.0.1";

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbBuilder" /> class.
    /// </summary>
    public CosmosDbBuilder()
        : this(new CosmosDbConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private CosmosDbBuilder(CosmosDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override CosmosDbConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the partition count.
    /// </summary>
    /// <param name="partitionCount">The number of partitions.</param>
    /// <returns>A configured instance of <see cref="CosmosDbBuilder" />.</returns>
    public CosmosDbBuilder WithPartitionCount(int partitionCount)
    {
        return Merge(DockerResourceConfiguration, new CosmosDbConfiguration(partitionCount: partitionCount))
            .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", partitionCount.ToString());
    }

    /// <summary>
    /// Sets the overridden IP address.
    /// </summary>
    /// <param name="ipAddress">The overridden IP address.</param>
    /// <returns>A configured instance of <see cref="CosmosDbBuilder" />.</returns>
    public CosmosDbBuilder WithIpAddressOverride(string ipAddress)
    {
        return Merge(DockerResourceConfiguration, new CosmosDbConfiguration(ipAddressOverride: ipAddress))
            .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE", ipAddress);
    }

    /// <inheritdoc />
    public override CosmosDbContainer Build()
    {
        Validate();
        return new CosmosDbContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Init()
    {
        return base.Init()
            .WithImage(CosmosDbEmulatorImage)
            .WithPortBinding(CosmosDbPort, true)
            .WithPartitionCount(DefaultPartitionCount)
            .WithIpAddressOverride(DefaultIpAddressOverride)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Started|Shutting"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        if (!DockerResourceConfiguration.PartitionCount.HasValue)
        {
            throw new ArgumentException($"'{nameof(DockerResourceConfiguration.PartitionCount)}' cannot be null.");
        }

        if (DockerResourceConfiguration.PartitionCount.Value < 1)
        {
            throw new ArgumentException($"'{nameof(DockerResourceConfiguration.PartitionCount)}' cannot be less than 1.");
        }
        
        _ = Guard.Argument(DockerResourceConfiguration.IpAddressOverride, nameof(DockerResourceConfiguration.IpAddressOverride))
            .NotNull()
            .NotEmpty();

        if (!IPAddress.TryParse(DockerResourceConfiguration.IpAddressOverride, out _))
        {
            throw new ArgumentException($"'{nameof(DockerResourceConfiguration.IpAddressOverride)}' must be a valid IP address.");
        }
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CosmosDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CosmosDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Merge(CosmosDbConfiguration oldValue, CosmosDbConfiguration newValue)
    {
        return new CosmosDbBuilder(new CosmosDbConfiguration(oldValue, newValue));
    }
}