namespace Testcontainers.Triton;

public sealed class TritonContainerTest : IAsyncLifetime
{
    private readonly TritonContainer _tritonContainer;

    public TritonContainerTest()
    {
        var modelRepositoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models");

        _tritonContainer = new TritonBuilder(TestSession.GetImageFromDockerfile())
            .WithCommand("tritonserver", "--model-repository=/models")
            .WithResourceMapping(modelRepositoryPath, "/models/")
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _tritonContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _tritonContainer.DisposeAsync()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HealthReadyReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_tritonContainer.GetHttpEndpoint());

        // When
        using var httpResponse = await httpClient.GetAsync("/v2/health/ready", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task MetricsEndpointReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(new UriBuilder(Uri.UriSchemeHttp, _tritonContainer.Hostname, _tritonContainer.GetMappedPublicPort(TritonBuilder.TritonMetricsPort)).ToString());

        // When
        using var httpResponse = await httpClient.GetAsync("/metrics", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GrpcHealthCheckReturnsServing()
    {
        // Given
        using var channel = GrpcChannel.ForAddress(_tritonContainer.GetGrpcEndpoint());
        var client = new Health.HealthClient(channel);

        // When
        var response = await client.CheckAsync(new HealthCheckRequest(), cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HealthCheckResponse.Types.ServingStatus.Serving, response.Status);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetConnectionStringReturnsHttpEndpoint()
    {
        Assert.Equal(_tritonContainer.GetHttpEndpoint(), _tritonContainer.GetConnectionString());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task InferSimpleModelReturnsExpectedOutput()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_tritonContainer.GetHttpEndpoint());

        var requestBody = new StringContent(
            """
            {
              "inputs": [
                {
                  "name": "INPUT0",
                  "datatype": "FP32",
                  "shape": [1],
                  "data": [42.0]
                }
              ]
            }
            """,
            System.Text.Encoding.UTF8,
            "application/json");

        // When
        using var httpResponse = await httpClient.PostAsync("/v2/models/simple/infer", requestBody, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        var responseBody = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        Assert.Contains("OUTPUT0", responseBody);
        Assert.Contains("42", responseBody);
    }
}

public sealed class TritonBuilderValidationTest
{
    [Fact]
    public void BuildWithoutCommandThrowsArgumentException()
    {
        var builder = new TritonBuilder(TestSession.GetImageFromDockerfile());

        var exception = Assert.Throws<ArgumentException>(() => builder.Build());
        Assert.Contains("tritonserver", exception.Message);
    }
}
