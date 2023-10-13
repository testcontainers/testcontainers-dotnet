namespace Testcontainers.GCS;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class GCSBuilder : ContainerBuilder<GCSBuilder, GCSContainer, GCSConfiguration>
{
    public const string GCSImage = "fsouza/fake-gcs-server:1.47.5";
    public const ushort GCSPort = 4443;

    /// <summary>
    /// Initializes a new instance of the <see cref="GCSBuilder" /> class.
    /// </summary>
    public GCSBuilder()
        : this(new GCSConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GCSBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private GCSBuilder(GCSConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override GCSConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override GCSContainer Build()
    {
        Validate();
        return new GCSContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override GCSBuilder Init()
    {
        return base.Init()
            .WithImage(GCSImage)
            .WithPortBinding(GCSPort, true)
            .WithCommand("-scheme", "http")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(GCSPort).ForStatusCode(HttpStatusCode.NotFound)));
    }

    /// <inheritdoc />
    protected override GCSBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new GCSConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override GCSBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new GCSConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override GCSBuilder Merge(GCSConfiguration oldValue, GCSConfiguration newValue)
    {
        return new GCSBuilder(new GCSConfiguration(oldValue, newValue));
    }
}