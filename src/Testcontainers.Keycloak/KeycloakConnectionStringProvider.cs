namespace Testcontainers.Keycloak;

/// <summary>
/// Provides the Keycloak connection string.
/// </summary>
internal sealed class KeycloakConnectionStringProvider : ContainerConnectionStringProvider<KeycloakContainer, KeycloakConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBaseAddress();
    }
}