namespace Testcontainers.Azurite;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AzuriteBuilder : ContainerBuilder<AzuriteBuilder, AzuriteContainer, AzuriteConfiguration>
{
    public const string AzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.24.0";

    public const ushort BlobPort = 10000;

    public const ushort QueuePort = 10001;

    public const ushort TablePort = 10002;

    private readonly ISet<AzuriteService> _enabledServices = new HashSet<AzuriteService>();

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    public AzuriteBuilder()
        : this(new AzuriteConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;

        _enabledServices.Add(AzuriteService.Blob);
        _enabledServices.Add(AzuriteService.Queue);
        _enabledServices.Add(AzuriteService.Table);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private AzuriteBuilder(AzuriteConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override AzuriteConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override AzuriteContainer Build()
    {
        Validate();

        var waitStrategy = Wait.ForUnixContainer();

        if (_enabledServices.Contains(AzuriteService.Blob))
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Blob service is successfully listening");
        }

        if (_enabledServices.Contains(AzuriteService.Queue))
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Queue service is successfully listening");
        }

        if (_enabledServices.Contains(AzuriteService.Table))
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Table service is successfully listening");
        }

        var azuriteBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(waitStrategy);
        return new AzuriteContainer(azuriteBuilder.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Init()
    {
        return base.Init()
            .WithImage(AzuriteImage)
            .WithPortBinding(BlobPort, true)
            .WithPortBinding(QueuePort, true)
            .WithPortBinding(TablePort, true);
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Merge(AzuriteConfiguration oldValue, AzuriteConfiguration newValue)
    {
        return new AzuriteBuilder(new AzuriteConfiguration(oldValue, newValue));
    }
}