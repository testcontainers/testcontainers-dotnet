namespace Testcontainers.Keycloak;

/// <inheritdoc />
[PublicAPI]
public sealed class KeycloakContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public KeycloakContainer(KeycloakConfiguration configuration)
        : base(configuration) { }

    /// <summary>
    /// Gets the Keycloak base address.
    /// </summary>
    /// <returns>The Keycloak base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(KeycloakBuilder.KeycloakPort)
        ).ToString();
    }
}
