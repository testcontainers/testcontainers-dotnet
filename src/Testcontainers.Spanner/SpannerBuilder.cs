namespace Testcontainers.Spanner;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class SpannerBuilder : ContainerBuilder<SpannerBuilder, SpannerContainer, SpannerConfiguration>
{
    public const ushort SpannerGrpcPort = 9010;

    public const ushort SpannerHttpPort = 9020;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpannerBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>gcr.io/cloud-spanner-emulator/emulator:1.5.59</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://github.com/GoogleCloudPlatform/cloud-spanner-emulator/releases" />.
    /// <include file="SpannerBuilder.Generated.xml" path="doc/member[@name='ctor']/*" />
    /// </remarks>
    public SpannerBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpannerBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://github.com/GoogleCloudPlatform/cloud-spanner-emulator/releases" />.
    /// <include file="SpannerBuilder.Generated.xml" path="doc/member[@name='ctor']/*" />
    /// </remarks>
    public SpannerBuilder(IImage image)
        : this(new SpannerConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpannerBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private SpannerBuilder(SpannerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override SpannerConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override SpannerContainer Build()
    {
        Validate();
        return new SpannerContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override SpannerBuilder Init()
    {
        // The gateway logs "Cloud Spanner emulator running." after the gRPC server
        // answers requests, as it starts the REST server.
        return base.Init()
            .WithPortBinding(SpannerGrpcPort, true)
            .WithPortBinding(SpannerHttpPort, true)
            .WithConnectionStringProvider(() => new SpannerConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*Cloud Spanner emulator running\\..*"));
    }

    /// <inheritdoc />
    protected override SpannerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SpannerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SpannerBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SpannerConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SpannerBuilder Merge(SpannerConfiguration oldValue, SpannerConfiguration newValue)
    {
        return new SpannerBuilder(new SpannerConfiguration(oldValue, newValue));
    }
}