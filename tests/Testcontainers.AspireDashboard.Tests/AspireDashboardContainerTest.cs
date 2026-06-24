namespace Testcontainers.AspireDashboard;

public sealed class AspireDashboardContainerTest : IAsyncLifetime
{
    private readonly AspireDashboardContainer _aspireDashboardContainer = new AspireDashboardBuilder(TestSession.GetImageFromDockerfile()).Build();

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

        var address = new Uri(_aspireDashboardContainer.GetDashboardAddress());
        httpClient.BaseAddress = address;

        // When
        using var response = await httpClient.GetAsync("/", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}