namespace Testcontainers.Toxiproxy;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ToxiproxyBuilder : ContainerBuilder<ToxiproxyBuilder, ToxiproxyContainer, ToxiproxyConfiguration>
{
    public const string ToxiproxyImage = "ghcr.io/shopify/toxiproxy:2.12.0";

    public const ushort ToxiproxyPort = 8474;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
    /// </summary>
    public ToxiproxyBuilder()
        : this(new ToxiproxyConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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

    /// <summary>
    /// Adds an initial proxy that will be created automatically after the container starts.
    /// </summary>
    /// <param name="name">The proxy name.</param>
    /// <param name="listen">The listen address (e.g., 127.0.0.1:8888).</param>
    /// <param name="upstream">The upstream address (e.g., backend:80).</param>
    /// <returns>The builder instance.</returns>
    public ToxiproxyBuilder WithProxy(string name, string listen, string upstream)
    {
        return this;
    }

    /// <inheritdoc />
    public override ToxiproxyContainer Build()
    {
        Validate();
        return new ToxiproxyContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override ToxiproxyBuilder Init()
    {
        return base.Init()
            .WithImage(ToxiproxyImage)
            .WithPortBinding(ToxiproxyPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/proxies").ForPort(ToxiproxyPort)));
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