namespace Testcontainers.Keycloak.Tests;

public sealed class KeycloakContainerHealthTest
{

    [Theory]
    [InlineData("quay.io/keycloak/keycloak:24.0")]
    [InlineData("quay.io/keycloak/keycloak:25.0")]
    public async Task HealthIsReady(string image)
    {
        // Given
        var keycloakContainer = new KeycloakBuilder().WithImage(image).Build();
        await keycloakContainer.StartAsync()
            .ConfigureAwait(true);

        try
        {
            using var httpClient = new HttpClient();

            try { 
                httpClient.BaseAddress = new UriBuilder(Uri.UriSchemeHttp, keycloakContainer.Hostname, keycloakContainer.GetMappedPublicPort(KeycloakBuilder.KeycloakHealthPort)).Uri;
            }
            catch (Exception e)
            {
                httpClient.BaseAddress = new Uri(keycloakContainer.GetBaseAddress());
            }

            // When
            using var response = await httpClient.GetAsync("/health/ready")
                .ConfigureAwait(true);

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        finally
        {
            await keycloakContainer.DisposeAsync().AsTask()
                .ConfigureAwait(true);
        }
    }
}