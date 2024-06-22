namespace Testcontainers.AspireDashboard;

public sealed class AspireDashboardContainerTest : IAsyncLifetime
{
    private readonly AspireDashboardContainer _container = new AspireDashboardBuilder()
        .AllowAnonymous(true)
        .AllowUnsecuredTransport(false)
        .Build();

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task GetDashboardReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();

        var address = new Uri(_container.GetDashboardUrl());
        httpClient.BaseAddress = address;

        // When
        using var response = await httpClient.GetAsync("/");

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}