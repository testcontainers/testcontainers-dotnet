namespace Testcontainers.Keycloak.Tests;

public sealed class KeycloakContainerTest : ContainerTest<KeycloakBuilder, KeycloakContainer>
{
    [Fact]
    public async Task GetOpenIdEndpointReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Container.GetBaseAddress());

        // When
        using var response = await httpClient.GetAsync("/realms/master/.well-known/openid-configuration")
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task MasterRealmIsEnabled()
    {
        // Given
        var keycloakClient = new KeycloakClient(Container.GetBaseAddress(), KeycloakBuilder.DefaultUsername, KeycloakBuilder.DefaultPassword);

        // When
        var masterRealm = await keycloakClient.GetRealmAsync("master")
            .ConfigureAwait(false);

        // Then
        Assert.True(masterRealm.Enabled);
    }
}