namespace Testcontainers.Typesense;

public abstract class TypesenseContainerTest : IAsyncLifetime
{
    private readonly TypesenseContainer _typesenseContainer;

    private TypesenseContainerTest(TypesenseContainer typesenseContainer)
    {
        _typesenseContainer = typesenseContainer;
    }

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
    public async Task HealthyReturnsTrue()
    {
        // Given
        var node = _typesenseContainer.GetNode();

        using var httpClient = new HttpClient
        {
            BaseAddress = node.BaseAddress
        };

        httpClient.DefaultRequestHeaders.Add("X-TYPESENSE-API-KEY", node.ApiKey);

        // When
        var response = await httpClient.GetAsync("/health", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(response.IsSuccessStatusCode);

        // Parse the JSON response body
        var responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

        // Then
        Assert.True(jsonResponse.TryGetProperty("ok", out var okProperty) && okProperty.GetBoolean());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CollectionsEmpty()
    {
        // Given
        var node = _typesenseContainer.GetNode();

        using var httpClient = new HttpClient
        {
            BaseAddress = node.BaseAddress
        };

        httpClient.DefaultRequestHeaders.Add("X-TYPESENSE-API-KEY", node.ApiKey);

        // When
        var response = await httpClient.GetAsync("/collections", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(response.IsSuccessStatusCode);

        // Parse the JSON response body
        var responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

        // Then
        Assert.True(jsonResponse.ValueKind == JsonValueKind.Array && jsonResponse.GetArrayLength() == 0);
    }

    [UsedImplicitly]
    public sealed class TypesenseDefaultConfiguration : TypesenseContainerTest
    {
        public TypesenseDefaultConfiguration()
            : base(new TypesenseBuilder().Build())
        {
        }
    }
}
