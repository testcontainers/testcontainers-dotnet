namespace Testcontainers.Keycloak.Tests;

public sealed class KeycloakContainerTest : IAsyncLifetime
{
    private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder().Build();

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
        using var response = await httpClient.GetAsync("/realms/master/.well-known/openid-configuration")
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
}