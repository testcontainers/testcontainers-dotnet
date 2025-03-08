namespace Testcontainers.Keycloak;

public abstract class KeycloakContainerTest : IAsyncLifetime
{
    private readonly KeycloakContainer _keycloakContainer;

    private KeycloakContainerTest(KeycloakContainer keycloakContainer)
    {
        _keycloakContainer = keycloakContainer;
    }

    public Task InitializeAsync()
    {
        return _keycloakContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _keycloakContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task GetOpenIdEndpointReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_keycloakContainer.GetBaseAddress());

        // When
        using var httpResponse = await httpClient.GetAsync("/realms/master/.well-known/openid-configuration")
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public async Task MasterRealmIsEnabled()
    {
        // Given
        var keycloakClient = new KeycloakClient(_keycloakContainer.GetBaseAddress(), KeycloakBuilder.DefaultUsername, KeycloakBuilder.DefaultPassword);

        // When
        var masterRealm = await keycloakClient.GetRealmAsync("master")
            .ConfigureAwait(true);

        // Then
        Assert.True(masterRealm.Enabled);
    }

    [UsedImplicitly]
    public sealed class KeycloakDefaultConfiguration : KeycloakContainerTest
    {
        public KeycloakDefaultConfiguration()
            : base(new KeycloakBuilder().Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class KeycloakV25Configuration : KeycloakContainerTest
    {
        public KeycloakV25Configuration()
            : base(new KeycloakBuilder().WithImage("quay.io/keycloak/keycloak:25.0").Build())
        {
        }
    }
}