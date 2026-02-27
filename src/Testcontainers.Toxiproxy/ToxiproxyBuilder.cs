namespace Testcontainers.Toxiproxy;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ToxiproxyBuilder : ContainerBuilder<ToxiproxyBuilder, ToxiproxyContainer, ToxiproxyConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string ToxiproxyImage = "ghcr.io/shopify/toxiproxy:2.12.0";

    public const ushort ToxiproxyControlPort = 8474;

    public const ushort FirstProxiedPort = 8666;

    public const ushort LastProxiedPort = 8666 + 32;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public ToxiproxyBuilder()
        : this(ToxiproxyImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>ghcr.io/shopify/toxiproxy:2.12.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://github.com/Shopify/toxiproxy/pkgs/container/toxiproxy" />.
    /// </remarks>
    public ToxiproxyBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://github.com/Shopify/toxiproxy/pkgs/container/toxiproxy" />.
    /// </remarks>
    public ToxiproxyBuilder(IImage image)
        : this(new ToxiproxyConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ToxiproxyBuilder(ToxiproxyConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ToxiproxyConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override ToxiproxyContainer Build()
    {
        Validate();
        return new ToxiproxyContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override ToxiproxyBuilder Init()
    {
        const int count = LastProxiedPort - FirstProxiedPort;

        var toxiproxyBuilder = base.Init()
            .WithPortBinding(ToxiproxyControlPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/version").ForPort(ToxiproxyControlPort)));

        // Allows up to 32 ports to be proxied (arbitrary value). The ports are
        // exposed here, but whether Toxiproxy listens on them is controlled at
        // runtime when configuring the proxy.
        return Enumerable.Range(FirstProxiedPort, count)
            .Aggregate(toxiproxyBuilder, (builder, port) =>
                builder.WithPortBinding(port, true));
    }

    /// <inheritdoc />
    protected override ToxiproxyBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ToxiproxyConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ToxiproxyBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ToxiproxyConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ToxiproxyBuilder Merge(ToxiproxyConfiguration oldValue, ToxiproxyConfiguration newValue)
    {
        return new ToxiproxyBuilder(new ToxiproxyConfiguration(oldValue, newValue));
    }
}