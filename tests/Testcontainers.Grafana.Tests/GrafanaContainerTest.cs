namespace Testcontainers.Grafana;

public sealed class GrafanaContainerTest : IAsyncLifetime
{
    private readonly GrafanaContainer _grafanaContainer = new GrafanaBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _grafanaContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _grafanaContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HealthEndpointReturnsOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_grafanaContainer.GetHttpEndpoint());

        // When
        using var httpResponse = await httpClient.GetAsync("api/health", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task AuthenticatedRequestReturnsOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_grafanaContainer.GetHttpEndpoint());
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes($"{GrafanaBuilder.DefaultUsername}:{GrafanaBuilder.DefaultPassword}")));

        // When
        using var httpResponse = await httpClient.GetAsync("api/org", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetConnectionStringReturnsExpectedFormat()
    {
        // When
        var connectionString = _grafanaContainer.GetConnectionString();

        // Then
        Assert.NotNull(connectionString);
        Assert.Contains(GrafanaBuilder.DefaultUsername, connectionString);
        Assert.Contains(GrafanaBuilder.DefaultPassword, connectionString);
        Assert.Contains(_grafanaContainer.Hostname, connectionString);
    }

    public sealed class WithCustomCredentials : IAsyncLifetime
    {
        private readonly GrafanaContainer _grafanaContainer = new GrafanaBuilder()
            .WithUsername("custom-user")
            .WithPassword("custom-pass")
            .Build();

        public async ValueTask InitializeAsync()
        {
            await _grafanaContainer.StartAsync()
                .ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            return _grafanaContainer.DisposeAsync();
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task AuthenticatedRequestWithCustomCredentialsReturnsOk()
        {
            // Given
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_grafanaContainer.GetHttpEndpoint());
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes("custom-user:custom-pass")));

            // When
            using var httpResponse = await httpClient.GetAsync("api/org", TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }
    }

    public sealed class WithAnonymousAccess : IAsyncLifetime
    {
        private readonly GrafanaContainer _grafanaContainer = new GrafanaBuilder()
            .WithAnonymousAccessEnabled()
            .Build();

        public async ValueTask InitializeAsync()
        {
            await _grafanaContainer.StartAsync()
                .ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            return _grafanaContainer.DisposeAsync();
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task AnonymousRequestReturnsOk()
        {
            // Given
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_grafanaContainer.GetHttpEndpoint());

            // When
            using var httpResponse = await httpClient.GetAsync("api/org", TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }
    }
}