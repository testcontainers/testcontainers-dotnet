namespace Testcontainers.Elasticsearch;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ElasticsearchBuilder : ContainerBuilder<ElasticsearchBuilder, ElasticsearchContainer, ElasticsearchConfiguration>
{
    public const string ElasticsearchVmOptionsDirectoryPath = "/usr/share/elasticsearch/config/jvm.options.d/";

    public const string ElasticsearchDefaultMemoryVmOptionFileName = "elasticsearch-default-memory-vm.options";

    public const string ElasticsearchDefaultMemoryVmOptionFilePath = ElasticsearchVmOptionsDirectoryPath + ElasticsearchDefaultMemoryVmOptionFileName;

    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string ElasticsearchImage = "elasticsearch:8.6.1";

    public const ushort ElasticsearchHttpsPort = 9200;

    public const ushort ElasticsearchTcpPort = 9300;

    public const string DefaultUsername = "elastic";

    public const string DefaultPassword = "elastic";

    private static readonly byte[] DefaultMemoryVmOption = Encoding.Default.GetBytes(string.Join("\n", "-Xms2147483648", "-Xmx2147483648"));

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public ElasticsearchBuilder()
        : this(ElasticsearchImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>elasticsearch:8.6.1</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/elasticsearch/tags" />.
    /// </remarks>
    public ElasticsearchBuilder(string image)
        : this(new ElasticsearchConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchBuilder" /> class.
    /// </summary>
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/elasticsearch/tags" />.
    /// </remarks>
    public ElasticsearchBuilder(IImage image)
        : this(new ElasticsearchConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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

        // By default, the base builder waits until the container is running. However, for Elasticsearch, a more advanced waiting strategy is necessary that requires access to the username and password.
        // If the user does not provide a custom waiting strategy, append the default Elasticsearch waiting strategy.
        var elasticsearchBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil(DockerResourceConfiguration)));
        return new ElasticsearchContainer(elasticsearchBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override ElasticsearchBuilder Init()
    {
        return base.Init()
            .WithPortBinding(ElasticsearchHttpsPort, true)
            .WithPortBinding(ElasticsearchTcpPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithEnvironment("discovery.type", "single-node")
            .WithEnvironment("ingest.geoip.downloader.enabled", "false")
            .WithResourceMapping(DefaultMemoryVmOption, ElasticsearchDefaultMemoryVmOptionFilePath);
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
        private readonly bool _tlsEnabled;

        private readonly string _authToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntil" /> class.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        public WaitUntil(ElasticsearchConfiguration configuration)
        {
            var username = configuration.Username;
            var password = configuration.Password;
            _tlsEnabled = configuration.TlsEnabled;
            _authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", username, password)));
        }

        private static async Task<bool> IsNodeReadyAsync(HttpResponseMessage response)
        {
            const StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;

            // https://www.elastic.co/docs/api/doc/elasticsearch/operation/operation-cluster-health.
            var jsonString = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            try
            {
                var status = JsonDocument.Parse(jsonString)
                    .RootElement
                    .GetProperty("status")
                    .GetString();

                return "green".Equals(status, comparisonType) || "yellow".Equals(status, comparisonType);
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc cref="IWaitUntil.UntilAsync" />
        public async Task<bool> UntilAsync(IContainer container)
        {
            using var httpMessageHandler = new HttpClientHandler();
            httpMessageHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            var httpWaitStrategy = new HttpWaitStrategy()
                .UsingHttpMessageHandler(httpMessageHandler)
                .UsingTls(_tlsEnabled)
                .ForPort(ElasticsearchHttpsPort)
                .ForPath("/_cluster/health")
                .WithHeader("Authorization", "Basic " + _authToken)
                .ForResponseMessageMatching(IsNodeReadyAsync);

            return await httpWaitStrategy.UntilAsync(container)
                .ConfigureAwait(false);
        }
    }
}