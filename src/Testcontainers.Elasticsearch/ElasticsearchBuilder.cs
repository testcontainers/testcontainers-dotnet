namespace Testcontainers.Elasticsearch;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ElasticsearchBuilder : ContainerBuilder<ElasticsearchBuilder, ElasticsearchContainer, ElasticsearchConfiguration>
{
    public const string ElasticsearchVmOptionsDirectoryPath = "/usr/share/elasticsearch/config/jvm.options.d/";

    public const string ElasticsearchDefaultMemoryVmOptionFileName = "elasticsearch-default-memory-vm.options";

    public const string ElasticsearchDefaultMemoryVmOptionFilePath = ElasticsearchVmOptionsDirectoryPath + ElasticsearchDefaultMemoryVmOptionFileName;

    public const string ElasticsearchImage = "elasticsearch:8.6.1";

    public const ushort ElasticsearchHttpsPort = 9200;

    public const ushort ElasticsearchTcpPort = 9300;

    public const string DefaultUsername = "elastic";

    public const string DefaultPassword = "elastic";

    private static readonly byte[] DefaultMemoryVmOption = Encoding.Default.GetBytes(string.Join("\n", "-Xms2147483648", "-Xmx2147483648"));

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchBuilder" /> class.
    /// </summary>
    public ElasticsearchBuilder()
        : this(new ElasticsearchConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ElasticsearchBuilder(ElasticsearchConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ElasticsearchConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Elasticsearch password.
    /// </summary>
    /// <param name="password">The Elasticsearch password.</param>
    /// <returns>A configured instance of <see cref="ElasticsearchBuilder" />.</returns>
    public ElasticsearchBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new ElasticsearchConfiguration(password: password))
            .WithEnvironment("ELASTIC_PASSWORD", password);
    }

    /// <inheritdoc />
    public override ElasticsearchContainer Build()
    {
        Validate();
        return new ElasticsearchContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override ElasticsearchBuilder Init()
    {
        return base.Init()
            .WithImage(ElasticsearchImage)
            .WithPortBinding(ElasticsearchHttpsPort, true)
            .WithPortBinding(ElasticsearchTcpPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithEnvironment("discovery.type", "single-node")
            .WithEnvironment("ingest.geoip.downloader.enabled", "false")
            .WithResourceMapping(DefaultMemoryVmOption, ElasticsearchDefaultMemoryVmOptionFilePath)
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override ElasticsearchBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ElasticsearchConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ElasticsearchBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ElasticsearchConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ElasticsearchBuilder Merge(ElasticsearchConfiguration oldValue, ElasticsearchConfiguration newValue)
    {
        return new ElasticsearchBuilder(new ElasticsearchConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Sets the Elasticsearch username.
    /// </summary>
    /// <remarks>
    /// The Docker image does not allow to configure the username.
    /// </remarks>
    /// <param name="username">The Elasticsearch username.</param>
    /// <returns>A configured instance of <see cref="ElasticsearchBuilder" />.</returns>
    private ElasticsearchBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new ElasticsearchConfiguration(username: username));
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        private static readonly IEnumerable<string> Pattern = new[] { "\"message\":\"started", "\"message\": \"started\"" };

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var (stdout, _) = await container.GetLogsAsync(since: container.StoppedTime, timestampsEnabled: false)
                .ConfigureAwait(false);

            return Pattern.Any(stdout.Contains);
        }
    }
}