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
        using var httpClientFactory = await ElasticsearchHttpClientFactory.CreateAsync(_elasticsearchContainer, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var httpClient = httpClientFactory.CreateHttpClient();

        var spansJson = string.Empty;

        var serviceName = Guid.NewGuid().ToString("D");

        var spanName = Guid.NewGuid().ToString("D");

        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);

        var otlpExporterConfiguration = new Dictionary<string, string>
        {
            { "OTEL_EXPORTER_OTLP_ENDPOINT", _elasticsearchContainer.GetOtlpEndpoint() },
            { "OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(otlpExporterConfiguration)
            .Build();

        var tracerProviderBuilder = Sdk
            .CreateTracerProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddSource(serviceName)
            .AddOtlpExporter(options => options.HttpClientFactory = () => httpClient)
            .ConfigureServices(services => services.AddSingleton<IConfiguration>(configuration));

        // When
        using (var _ = tracerProviderBuilder.Build())
        {
            using var activitySource = new ActivitySource(serviceName);
            using var activity = activitySource.StartActivity(spanName);
            activity.SetTag("test.key", "test-value");
        }

        var spanIsIndexed = async () =>
        {
            using var httpResponseMessage = await httpClient.PostAsync("/traces-*/_refresh", null, TestContext.Current.CancellationToken)
                .ConfigureAwait(false);

            spansJson = await httpClient.GetStringAsync("/traces-*/_search", TestContext.Current.CancellationToken)
                .ConfigureAwait(false);

            return spansJson.Contains(spanName);
        };

        await WaitStrategy.WaitUntilAsync(spanIsIndexed, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1), ct: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Contains(serviceName, spansJson);
        Assert.Contains(spanName, spansJson);
    }
    // # --8<-- [end:UseElasticsearchOtlpEndpoint]
}