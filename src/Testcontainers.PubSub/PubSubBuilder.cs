namespace Testcontainers.PubSub;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PubSubBuilder : ContainerBuilder<PubSubBuilder, PubSubContainer, PubSubConfiguration>
{
    public const string GoogleCloudCliImage = "gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators";

    public const ushort PubSubPort = 8085;

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubBuilder" /> class.
    /// </summary>
    public PubSubBuilder()
        : this(new PubSubConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private PubSubBuilder(PubSubConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override PubSubConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override PubSubContainer Build()
    {
        Validate();
        return new PubSubContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override PubSubBuilder Init()
    {
        return base.Init()
            .WithImage(GoogleCloudCliImage)
            .WithPortBinding(PubSubPort, true)
            .WithEntrypoint("gcloud")
            .WithCommand("beta", "emulators", "pubsub", "start", "--host-port", "0.0.0.0:" + PubSubPort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*started.*$"));
    }

    /// <inheritdoc />
    protected override PubSubBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PubSubConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PubSubBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PubSubConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PubSubBuilder Merge(PubSubConfiguration oldValue, PubSubConfiguration newValue)
    {
        return new PubSubBuilder(new PubSubConfiguration(oldValue, newValue));
    }
}