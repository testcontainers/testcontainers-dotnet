namespace Testcontainers.WireMock;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class WireMockBuilder : ContainerBuilder<WireMockBuilder, WireMockContainer, WireMockConfiguration>
{
    public const string WireMockImage = "wiremock/wiremock:3.5.4";

    public const ushort WireMockPort = 8080;

    private readonly List<string> _mappingFiles = new();
    private readonly List<string> _staticFiles = new();
    private readonly List<string> _extensions = new();
    private readonly List<string> _cliArgs = new();
    private bool _disableBanner;

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockBuilder" /> class.
    /// </summary>
    public WireMockBuilder()
        : this(new WireMockConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private WireMockBuilder(WireMockConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override WireMockConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets mapping JSON files for WireMock.
    /// </summary>
    /// <param name="mappingPath">Path to mapping JSON file.</param>
    /// <returns>A configured instance of <see cref="WireMockBuilder" />.</returns>
    public WireMockBuilder WithMapping(string mappingPath)
    {
        _mappingFiles.Add(mappingPath);
        return this;
    }

    /// <summary>
    /// Sets static files to be served by WireMock.
    /// </summary>
    /// <param name="filePath">Path to static file.</param>
    /// <returns>A configured instance of <see cref="WireMockBuilder" />.</returns>
    public WireMockBuilder WithStaticFile(string filePath)
    {
        _staticFiles.Add(filePath);
        return this;
    }

    /// <summary>
    /// Adds WireMock extensions.
    /// </summary>
    /// <param name="extensionClassName">Full class name of the extension.</param>
    /// <returns>A configured instance of <see cref="WireMockBuilder" />.</returns>
    public WireMockBuilder WithExtension(string extensionClassName)
    {
        _extensions.Add(extensionClassName);
        return this;
    }

    /// <summary>
    /// Disables the WireMock startup banner.
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockBuilder" />.</returns>
    public WireMockBuilder WithoutBanner()
    {
        _disableBanner = true;
        return this;
    }

    /// <summary>
    /// Adds custom CLI arguments to WireMock.
    /// </summary>
    /// <param name="arg">CLI argument.</param>
    /// <returns>A configured instance of <see cref="WireMockBuilder" />.</returns>
    public WireMockBuilder WithCliArg(string arg)
    {
        _cliArgs.Add(arg);
        return this;
    }

    /// <inheritdoc />
    public override WireMockContainer Build()
    {
        // Apply custom configurations
        var builder = this;

        // Add mapping files
        foreach (var mappingFile in _mappingFiles)
        {
            var containerPath = $"/home/wiremock/mappings/{Path.GetFileName(mappingFile)}";
            builder = builder.WithResourceMapping(new FileInfo(mappingFile), new FileInfo(containerPath));
        }

        // Add static files
        foreach (var staticFile in _staticFiles)
        {
            var containerPath = $"/home/wiremock/__files/{Path.GetFileName(staticFile)}";
            builder = builder.WithResourceMapping(new FileInfo(staticFile), new FileInfo(containerPath));
        }

        // Build command
        var command = new List<string>();
        
        if (_disableBanner)
        {
            command.Add("--disable-banner");
        }

        if (_extensions.Count > 0)
        {
            command.Add("--extensions");
            command.Add(string.Join(",", _extensions));
        }

        command.AddRange(_cliArgs);

        if (command.Count > 0)
        {
            builder = builder.WithCommand(command.ToArray());
        }

        Validate();
        return new WireMockContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override WireMockBuilder Init()
    {
        return base.Init()
            .WithImage(WireMockImage)
            .WithPortBinding(WireMockPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r
                    .ForPath("/__admin/health")
                    .ForPort(WireMockPort)));
    }

    /// <inheritdoc />
    protected override WireMockBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new WireMockConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override WireMockBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new WireMockConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override WireMockBuilder Merge(WireMockConfiguration oldValue, WireMockConfiguration newValue)
    {
        return new WireMockBuilder(new WireMockConfiguration(oldValue, newValue));
    }
}