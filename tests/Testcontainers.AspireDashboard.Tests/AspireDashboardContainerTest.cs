namespace Testcontainers.AspireDashboard;

public sealed class AspireDashboardContainerTest : IAsyncLifetime
{
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
    public async Task GetDashboardReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_aspireDashboardContainer.GetDashboardAddress());

        // When
        using var response = await httpClient.GetAsync("/", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public Task OtlpExportOverGrpcSpanIsIngested()
    {
        var endpoint = new Uri(_aspireDashboardContainer.GetOtlpGrpcAddress());
        return ExportAndAssertSpanIngestedAsync(endpoint, OtlpExportProtocol.Grpc);
    }

    [Fact]
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

        Assert.Contains("\"totalCount\":1", spansJson);
        Assert.Contains(_serviceName, spansJson);
        Assert.Contains(_spanName, spansJson);
    }
}