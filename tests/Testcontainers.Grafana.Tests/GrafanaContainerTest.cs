namespace Testcontainers.Grafana;

public abstract class GrafanaContainerTest : IAsyncLifetime
{
    private readonly GrafanaContainer _grafanaContainer;

    private readonly string _username;

    private readonly string _password;

    private GrafanaContainerTest(GrafanaContainer grafanaContainer, string username, string password)
    {
        _grafanaContainer = grafanaContainer;
        _username = username;
        _password = password;
    }

    // # --8<-- [start:UseGrafanaContainer]
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
        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", _username, _password)));

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_grafanaContainer.GetBaseAddress());
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        // When
        using var httpResponse = await httpClient.GetAsync("api/org", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }
    // # --8<-- [end:UseGrafanaContainer]

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await _grafanaContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    // # --8<-- [start:CreateGrafanaContainer]
    [UsedImplicitly]
    public sealed class GrafanaDefaultConfiguration : GrafanaContainerTest
    {
        public GrafanaDefaultConfiguration()
            : base(new GrafanaBuilder(TestSession.GetImageFromDockerfile()).Build(), GrafanaBuilder.DefaultUsername, GrafanaBuilder.DefaultPassword)
        {
        }
    }

    [UsedImplicitly]
    public sealed class CustomCredentialsConfiguration : GrafanaContainerTest
    {
        private static readonly string Username = Guid.NewGuid().ToString("D");

        private static readonly string Password = Guid.NewGuid().ToString("D");

        public CustomCredentialsConfiguration()
            : base(new GrafanaBuilder(TestSession.GetImageFromDockerfile()).WithUsername(Username).WithPassword(Password).Build(), Username, Password)
        {
        }
    }

    [UsedImplicitly]
    public sealed class NoAuthCredentialsConfiguration : GrafanaContainerTest
    {
        public NoAuthCredentialsConfiguration()
            : base(new GrafanaBuilder(TestSession.GetImageFromDockerfile()).WithAnonymousAccessEnabled().Build(), string.Empty, string.Empty)
        {
        }
    }
    // # --8<-- [end:CreateGrafanaContainer]
}