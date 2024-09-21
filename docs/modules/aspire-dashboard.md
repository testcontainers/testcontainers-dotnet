# Aspire Dashboard

## Configuration

The Aspire Dashboard can be configured in the following ways:

- Use `AllowAnonymous(true)` to allow anonymous access to the dashboard.
- Use `AllowUnsecuredTransport` to allow unsecured transport, such as HTTP.
- Aspire Dashboard usually runs on port 18888. You can change the port by using the `WithPortBinding`

```csharp
public sealed class AspireDashboardContainerTest : IAsyncLifetime
{
    private readonly AspireDashboardContainer _container = new AspireDashboardBuilder()
        .AllowAnonymous(true)
        .AllowUnsecuredTransport(false)
        .WithPortBinding(
            AspireDashboardBuilder.AspireDashboardFrontendPort,
            AspireDashboardBuilder.AspireDashboardFrontendPort
        )
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
        using var httpClient = new HttpClient();
        var address = new Uri(_container.GetDashboardUrl());
        httpClient.BaseAddress = address;

        using var response = await httpClient.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```
[Aspire Dashboard on Microsoft Artifact Registry](https://mcr.microsoft.com/en-us/product/dotnet/aspire-dashboard/tags)
