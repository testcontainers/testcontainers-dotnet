namespace Testcontainers.Temporalio;

public sealed class TemporalContainerTest : IAsyncLifetime
{
    private readonly TemporalContainer _temporalContainer = new TemporalBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _temporalContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _temporalContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStringReturnsGrpcAddress()
    {
        Assert.Equal(_temporalContainer.GetGrpcAddress(), _temporalContainer.GetConnectionString());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task WebUiReturnsHttpOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_temporalContainer.GetWebUiAddress());

        // When
        using var response = await httpClient.GetAsync("/", CancellationToken.None).ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ListNamespacesReturnsDefaultNamespace()
    {
        // Given
        var client = await TemporalClient.ConnectAsync(new(_temporalContainer.GetGrpcAddress())
        {
            Namespace = "default",
        }).ConfigureAwait(true);

        // When
        var response = await client.Connection.WorkflowService.ListNamespacesAsync(new()).ConfigureAwait(true);

        // Then
        Assert.Contains(response.Namespaces, ns => ns.NamespaceInfo.Name == "default");
    }
}
