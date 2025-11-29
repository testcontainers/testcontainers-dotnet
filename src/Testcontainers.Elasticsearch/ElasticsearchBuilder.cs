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
        return new ElasticsearchContainer(DockerResourceConfiguration);
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
        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            if (container is not ElasticsearchContainer elasticContainer) return false;
            var containerCredentials = elasticContainer.GetCredentials();
            var username = containerCredentials.UserName ?? DefaultUsername;
            var password = containerCredentials.Password ?? DefaultPassword;
            var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            using var httpMessageHandler = new HttpClientHandler();
            httpMessageHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true; // intentionally trusting the self‑signed test certificate

            var httpWaitStrategy = new HttpWaitStrategy()
                .UsingHttpMessageHandler(httpMessageHandler)
                .UsingTls(elasticContainer.HttpsEnabled)
                .ForPath("/_cluster/health")
                .ForPort(ElasticsearchHttpsPort)
                .ForStatusCode(HttpStatusCode.OK)
                .WithHeader("Authorization", "Basic " + base64Credentials)
                .ForResponseMessageMatching(async m =>
                {
                    var content = await m.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        var response = JsonSerializer.Deserialize<ElasticHealthResponse>(content) ?? new ElasticHealthResponse();
                        return string.Equals(ElasticHealthResponse.YellowStatus, response.Status, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(ElasticHealthResponse.GreenStatus, response.Status, StringComparison.OrdinalIgnoreCase);
                    }
                    catch (JsonException)
                    {
                        return false;
                    }
                });

            return await httpWaitStrategy.UntilAsync(container)
                .ConfigureAwait(false);
        }

        private class ElasticHealthResponse
        {
            public const string YellowStatus = "yellow";
            public const string GreenStatus = "green";

            [JsonPropertyName("status")]
            public string Status { get; set; } = "unknown";
        }
    }
}