namespace Testcontainers.Keycloak;

/// <inheritdoc />
[PublicAPI]
public sealed class KeycloakContainer : DockerContainer
{
	private readonly KeycloakConfiguration _configuration;

	/// <summary>
	/// Initializes a new instance of the <see cref="KeycloakContainer"/> class.
	/// </summary>
	/// <param name="configuration">The container configuration.</param>
	/// <param name="logger">The logger.</param>
	public KeycloakContainer(KeycloakConfiguration configuration, ILogger logger)
		: base(configuration, logger)
	{
		_configuration = configuration;
	}

	/// <summary>
	/// Gets the base address for this Keycloak instance.
	/// </summary>
	/// <returns>The base address for this Keycloak instance</returns>
	public Uri GetBaseAddress()
	{
		return new UriBuilder
		{
			Scheme = "http",
			Host = Hostname,
			Port = GetMappedPublicPort(KeycloakBuilder.KeycloakPort)
		}.Uri;
	}
}
