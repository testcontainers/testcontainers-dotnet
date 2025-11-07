namespace Testcontainers.Grafana;

public abstract class GrafanaContainerTest : IAsyncLifetime
{
    private readonly GrafanaContainer _grafanaContainer;

    private readonly string _username;

    private readonly string _password;

    private GrafanaContainerTest(
        GrafanaContainer grafanaContainer,
        string username,
        string password)
    {
        _grafanaContainer = grafanaContainer;
        _username = username;
        _password = password;
    }

    public async ValueTask InitializeAsync()
    {
        await _grafanaContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetCurrentOrganizationReturnsHttpStatusCodeOk()
    {
        // Given
        var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Join(":", _username, _password)));

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_grafanaContainer.GetBaseAddress());
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

        // When
        using var httpResponse = await httpClient.GetAsync("api/org", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await _grafanaContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [UsedImplicitly]
    public sealed class GrafanaDefaultConfiguration : GrafanaContainerTest
    {
        public GrafanaDefaultConfiguration()
            : base(new GrafanaBuilder().Build(), GrafanaBuilder.DefaultUsername, GrafanaBuilder.DefaultPassword)
        {
        }
    }

    [UsedImplicitly]
    public sealed class CustomCredentialsConfiguration : GrafanaContainerTest
    {
        private static readonly string Username = Guid.NewGuid().ToString("D");

        private static readonly string Password = Guid.NewGuid().ToString("D");

        public CustomCredentialsConfiguration()
            : base(new GrafanaBuilder().WithUsername(Username).WithPassword(Password).Build(), Username, Password)
        {
        }
    }

    [UsedImplicitly]
    public sealed class NoAuthCredentialsConfiguration : GrafanaContainerTest
    {
        public NoAuthCredentialsConfiguration()
            : base(new GrafanaBuilder().WithAnonymousAccessEnabled().Build(), string.Empty, string.Empty)
        {
        }
    }
}