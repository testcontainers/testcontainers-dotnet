namespace Testcontainers.Azurite;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AzuriteBuilder : ContainerBuilder<AzuriteBuilder, AzuriteContainer, AzuriteConfiguration>
{
    public const string AzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.28.0";

    public const ushort BlobPort = 10000;

    public const ushort QueuePort = 10001;

    public const ushort TablePort = 10002;

    private static readonly ISet<AzuriteService> EnabledServices = new HashSet<AzuriteService>();

    static AzuriteBuilder()
    {
        EnabledServices.Add(AzuriteService.Blob);
        EnabledServices.Add(AzuriteService.Queue);
        EnabledServices.Add(AzuriteService.Table);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    public AzuriteBuilder()
        : this(new AzuriteConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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

    /// <summary>
    /// Enables in-memory persistence.
    /// </summary>
    /// <remarks>
    /// By default, the in-memory is limited to 50% of the total memory on the container.
    /// </remarks>
    /// <param name="megabytes">An optional in-memory limit in megabytes.</param>
    /// <returns>A configured instance of <see cref="AzuriteBuilder" />.</returns>
    public AzuriteBuilder WithInMemoryPersistence(float? megabytes = null)
    {
        if (megabytes.HasValue)
        {
            return WithCommand("--inMemoryPersistence", "--extentMemoryLimit", megabytes.ToString());
        }
        else
        {
            return WithCommand("--inMemoryPersistence");
        }
    }

    /// <inheritdoc />
    public override AzuriteContainer Build()
    {
        Validate();

        var waitStrategy = Wait.ForUnixContainer();

        if (EnabledServices.Contains(AzuriteService.Blob))
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Blob service is successfully listening");
        }

        if (EnabledServices.Contains(AzuriteService.Queue))
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Queue service is successfully listening");
        }

        if (EnabledServices.Contains(AzuriteService.Table))
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
            .WithPortBinding(TablePort, true)
            .WithEntrypoint("azurite")
            .WithCommand("--blobHost", "0.0.0.0", "--queueHost", "0.0.0.0", "--tableHost", "0.0.0.0");
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