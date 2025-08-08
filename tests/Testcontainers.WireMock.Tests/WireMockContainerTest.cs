namespace Testcontainers.WireMock;

public sealed class WireMockContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseWireMockContainer]
    private readonly WireMockContainer _wireMockContainer = new WireMockBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _wireMockContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _wireMockContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task AdminHealthEndpointReturnsOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_wireMockContainer.GetBaseUrl());

        // When
        var response = await httpClient.GetAsync("__admin/health")
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    // # --8<-- [end:UseWireMockContainer]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CanAddMappingAndReceiveStubResponse()
    {
        // Given
        const string mappingJson = @"{
            ""request"": {
                ""method"": ""GET"",
                ""url"": ""/hello""
            },
            ""response"": {
                ""status"": 200,
                ""body"": ""Hello from WireMock!"",
                ""headers"": {
                    ""Content-Type"": ""text/plain""
                }
            }
        }";

        // When
        var result = await _wireMockContainer.AddMappingFromJsonAsync(mappingJson)
            .ConfigureAwait(false);
        
        // Verify mapping was added successfully
        Assert.Equal(0, result.ExitCode);

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_wireMockContainer.GetBaseUrl());
        var response = await httpClient.GetAsync("hello")
            .ConfigureAwait(false);

        var content = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Hello from WireMock!", content);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CanResetMappings()
    {
        // Given
        const string mappingJson = @"{
            ""request"": {
                ""method"": ""GET"",
                ""url"": ""/test""
            },
            ""response"": {
                ""status"": 200,
                ""body"": ""Test response""
            }
        }";

        var addResult = await _wireMockContainer.AddMappingFromJsonAsync(mappingJson)
            .ConfigureAwait(false);
        Assert.Equal(0, addResult.ExitCode);

        // When
        var execResult = await _wireMockContainer.ResetAsync()
            .ConfigureAwait(false);

        // Then
        Assert.Equal(0, execResult.ExitCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CanGetRequestCount()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_wireMockContainer.GetBaseUrl());
        await httpClient.GetAsync("test")
            .ConfigureAwait(false);

        // When
        var execResult = await _wireMockContainer.GetRequestCountAsync()
            .ConfigureAwait(false);

        // Then
        Assert.Equal(0, execResult.ExitCode);
        Assert.Contains("count", execResult.Stdout);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetBaseUrlReturnsCorrectUrl()
    {
        // When
        var baseUrl = _wireMockContainer.GetBaseUrl();

        // Then
        Assert.NotNull(baseUrl);
        Assert.StartsWith("http://", baseUrl);
        Assert.Contains(_wireMockContainer.Hostname, baseUrl);
    }
}