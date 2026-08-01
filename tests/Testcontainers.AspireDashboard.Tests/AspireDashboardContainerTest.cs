namespace Testcontainers.AspireDashboard;

public sealed class AspireDashboardContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseAspireDashboardContainer]
    private readonly AspireDashboardContainer _aspireDashboardContainer = new AspireDashboardBuilder(TestSession.GetImageFromDockerfile()).Build();

    private readonly string _serviceName = Guid.NewGuid().ToString("N");

    private readonly string _spanName = Guid.NewGuid().ToString("N");

    public async ValueTask InitializeAsync()
    {
        await _aspireDashboardContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _aspireDashboardContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetDashboardReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_aspireDashboardContainer.GetDashboardAddress());

        // When
        using var httpResponse = await httpClient.GetAsync("/", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal(_aspireDashboardContainer.GetDashboardAddress(), _aspireDashboardContainer.GetConnectionString());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public Task OtlpExportOverGrpcSpanIsIngested()
    {
        var endpoint = new Uri(_aspireDashboardContainer.GetOtlpGrpcAddress());
        return ExportAndAssertSpanIngestedAsync(endpoint, OtlpExportProtocol.Grpc);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public Task OtlpExportOverHttpSpanIsIngested()
    {
        var endpoint = new Uri(new Uri(_aspireDashboardContainer.GetOtlpHttpAddress()), "/v1/traces");
        return ExportAndAssertSpanIngestedAsync(endpoint, OtlpExportProtocol.HttpProtobuf);
    }

    private async Task ExportAndAssertSpanIngestedAsync(Uri endpoint, OtlpExportProtocol protocol)
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_aspireDashboardContainer.GetDashboardAddress());

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(_serviceName);

        var tracerProviderBuilder = Sdk
            .CreateTracerProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddSource(_serviceName)
            .AddOtlpExporter(options =>
            {
                options.Endpoint = endpoint;
                options.Protocol = protocol;
            });

        // When
        using (var _ = tracerProviderBuilder.Build())
        {
            using (var activitySource = new ActivitySource(_serviceName))
            {
                using (var activity = activitySource.StartActivity(_spanName))
                {
                    activity!.SetTag("test.key", "test-value");
                }
            }
        }

        // Then
        var spansJson = await httpClient.GetStringAsync("/api/telemetry/spans")
            .ConfigureAwait(true);

        using var jsonDocument = JsonDocument.Parse(spansJson);
        var totalCount = jsonDocument.RootElement.GetProperty("totalCount").GetInt32();

        Assert.Equal(1, totalCount);
        Assert.Contains(_serviceName, spansJson);
        Assert.Contains(_spanName, spansJson);
    }
    // # --8<-- [end:UseAspireDashboardContainer]
}