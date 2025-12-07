namespace Testcontainers.Typesense;

public sealed class TypesenseContainerTest : IAsyncLifetime
{
    private readonly TypesenseContainer _typesenseContainer = new TypesenseBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _typesenseContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _typesenseContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetCollectionsReturnsEmptyArray()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-TYPESENSE-API-KEY", TypesenseBuilder.DefaultApiKey);
        httpClient.BaseAddress = new Uri(_typesenseContainer.GetBaseAddress());

        // When
        using var httpResponse = await httpClient.GetAsync("/collections", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var response = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal("[]", response);
    }
}