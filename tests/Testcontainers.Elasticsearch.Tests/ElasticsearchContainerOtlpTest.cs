namespace Testcontainers.Elasticsearch;

public sealed class ElasticsearchContainerOtlpTest : IAsyncLifetime
{
    private readonly ElasticsearchContainer _elasticsearchContainer = new ElasticsearchBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _elasticsearchContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _elasticsearchContainer.DisposeAsync();
    }

    // # --8<-- [start:UseElasticsearchOtlpEndpoint]
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task OtlpExportSpanIsIngested()
    {
        // Given
        var serviceName = Guid.NewGuid().ToString("D");

        var spanName = Guid.NewGuid().ToString("D");

        using var caCertificate = await _elasticsearchContainer.GetCertificateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var httpMessageHandler = new HttpClientHandler();
        httpMessageHandler.ServerCertificateCustomValidationCallback = CertificateValidations.AuthorityIsRoot(caCertificate);

        var connectionString = new Uri(_elasticsearchContainer.GetConnectionString());

        var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.UnescapeDataString(connectionString.UserInfo))));

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(serviceName);

        var otlpExporterConfiguration = new Dictionary<string, string>();
        otlpExporterConfiguration.Add("OTEL_EXPORTER_OTLP_ENDPOINT", _elasticsearchContainer.GetOtlpEndpoint());
        otlpExporterConfiguration.Add("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(otlpExporterConfiguration)
            .Build();

        var tracerProviderBuilder = Sdk
            .CreateTracerProviderBuilder()
            .ConfigureServices(services => services.AddSingleton<IConfiguration>(configuration))
            .SetResourceBuilder(resourceBuilder)
            .AddSource(serviceName)
            .AddOtlpExporter(options =>
            {
                options.HttpClientFactory = () =>
                {
                    var exporterHttpClient = new HttpClient(httpMessageHandler, false);
                    exporterHttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                    return exporterHttpClient;
                };
            });

        // When
        using (var _ = tracerProviderBuilder.Build())
        {
            using var activitySource = new ActivitySource(serviceName);
            using var activity = activitySource.StartActivity(spanName);
            activity.SetTag("test.key", "test-value");
        }

        // Then
        using var httpClient = new HttpClient(httpMessageHandler, false);
        httpClient.BaseAddress = connectionString;
        httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

        var spansJson = string.Empty;

        await WaitStrategy.WaitWhileAsync(async () =>
            {
                using var httpResponseMessage = await httpClient.PostAsync("/traces-*/_refresh", null, TestContext.Current.CancellationToken)
                    .ConfigureAwait(false);

                spansJson = await httpClient.GetStringAsync("/traces-*/_search", TestContext.Current.CancellationToken)
                    .ConfigureAwait(false);

                return !spansJson.Contains(spanName);
            }, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1), ct: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        Assert.Contains(serviceName, spansJson);
        Assert.Contains(spanName, spansJson);
    }
    // # --8<-- [end:UseElasticsearchOtlpEndpoint]
}