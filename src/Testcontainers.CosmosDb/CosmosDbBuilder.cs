namespace Testcontainers.CosmosDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class CosmosDbBuilder : ContainerBuilder<CosmosDbBuilder, CosmosDbContainer, CosmosDbConfiguration>
{
    public const string CosmosDbImage = "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest";

    public const ushort CosmosDbPort = 8081;

    public const string DefaultAccountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

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
            .WithImage(CosmosDbImage)
            .WithPortBinding(CosmosDbPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Started\\r?\\n"));
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