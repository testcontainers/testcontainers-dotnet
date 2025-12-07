namespace Testcontainers.PubSub;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PubSubBuilder : ContainerBuilder<PubSubBuilder, PubSubContainer, PubSubConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string GoogleCloudCliImage = "gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators";

    public const ushort PubSubPort = 8085;

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public PubSubBuilder()
        : this(GoogleCloudCliImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://console.cloud.google.com/artifacts/docker/google.com:cloudsdktool/us/gcr.io/google-cloud-cli" />.
    /// </remarks>
    public PubSubBuilder(string image)
        : this(new PubSubConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://console.cloud.google.com/artifacts/docker/google.com:cloudsdktool/us/gcr.io/google-cloud-cli" />.
    /// </remarks>
    public PubSubBuilder(IImage image)
        : this(new PubSubConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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
        return new PubSubContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override PubSubBuilder Init()
    {
        return base.Init()
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