using Toxiproxy.Net;

namespace Testcontainers.Toxiproxy;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ToxiproxyBuilder : ContainerBuilder<ToxiproxyBuilder, ToxiproxyContainer, ToxiproxyConfiguration>
{
    public const string ToxiproxyImage = "ghcr.io/shopify/toxiproxy";
    public const ushort ControlPort = 8474;

    private readonly List<Proxy> _initialProxies = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
    /// </summary>
    private ToxiproxyBuilder(ToxiproxyConfiguration resourceConfiguration, List<Proxy> initialProxies)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
        _initialProxies = initialProxies;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ToxiproxyBuilder()
        : this(new ToxiproxyConfiguration(), new List<Proxy>())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }
    
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
        return new ToxiproxyContainer(DockerResourceConfiguration, _initialProxies);
    }

    /// <summary>
    /// Initialize the default Toxiproxy configuration with image, port, and wait strategy.
    /// </summary>
    /// <returns>A configured instance of <see cref="ToxiproxyBuilder" />.</returns>
    protected override ToxiproxyBuilder Init()
    {
        // Define a wait strategy that waits for the Toxiproxy HTTP API to respond with 200 OK at /proxies.
        return base.Init()
            .WithImage(ToxiproxyImage) // Set the Toxiproxy image.
            .WithPortBinding(ControlPort, true) // Bind the control port.
            .WithWaitStrategy(Wait.ForUnixContainer() // Use HTTP-based wait strategy.
                .UntilHttpRequestIsSucceeded(request => request
                    .ForPort(ControlPort)
                    .ForPath("/proxies")
                    .ForStatusCode(System.Net.HttpStatusCode.OK)));
    }

    /// <summary>
    /// Adds an initial proxy that will be created automatically after the container starts.
    /// </summary>
    /// <param name="name">The proxy name.</param>
    /// <param name="listen">The listen address (e.g., 127.0.0.1:8888).</param>
    /// <param name="upstream">The upstream address (e.g., backend:80).</param>
    /// <returns>The builder instance.</returns>
    public ToxiproxyBuilder WithProxy(string name, string listen, string upstream)
    {
        _initialProxies.Add(new Proxy
        {
            Name = name,
            Enabled = true,
            Listen = listen,
            Upstream = upstream
        });

        return this;
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();
        _ = Guard.Argument(DockerResourceConfiguration, nameof(DockerResourceConfiguration)).NotNull();
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
        var mergedConfiguration = new ToxiproxyConfiguration(oldValue, newValue);
        return new ToxiproxyBuilder(mergedConfiguration, new List<Proxy>(_initialProxies));
    }
}
