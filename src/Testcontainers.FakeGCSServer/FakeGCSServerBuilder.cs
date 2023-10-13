namespace Testcontainers.FakeGCSServer;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class FakeGCSServerBuilder : ContainerBuilder<FakeGCSServerBuilder, FakeGCSServerContainer, FakeGCSServerConfiguration>
{
    public const string FakeGCSServerImage = "fsouza/fake-gcs-server:1.47.5";
    public const ushort FakeGCSServerPort = 4443;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGCSServerBuilder" /> class.
    /// </summary>
    public FakeGCSServerBuilder()
        : this(new FakeGCSServerConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGCSServerBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private FakeGCSServerBuilder(FakeGCSServerConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override FakeGCSServerConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override FakeGCSServerContainer Build()
    {
        Validate();
        return new FakeGCSServerContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override FakeGCSServerBuilder Init()
    {
        return base.Init()
            .WithImage(FakeGCSServerImage)
            .WithPortBinding(FakeGCSServerPort, true)
            .WithCommand("-scheme", "http")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(FakeGCSServerPort).ForStatusCode(HttpStatusCode.NotFound)));
    }

    /// <inheritdoc />
    protected override FakeGCSServerBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FakeGCSServerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FakeGCSServerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FakeGCSServerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FakeGCSServerBuilder Merge(FakeGCSServerConfiguration oldValue, FakeGCSServerConfiguration newValue)
    {
        return new FakeGCSServerBuilder(new FakeGCSServerConfiguration(oldValue, newValue));
    }
}