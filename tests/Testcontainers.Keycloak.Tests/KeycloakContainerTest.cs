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
	public async Task GetBaseAddressReturnsValidAddress()
	{
		// Given
		using var httpClient = new HttpClient { BaseAddress = _keycloakContainer.GetBaseAddress() };

		// When
		using var response = await httpClient.GetAsync("/realms/master/.well-known/openid-configuration");

		// Then
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task MasterRealmIsCreatedAndEnabled()
	{
		// Given
		var baseAddress = _keycloakContainer.GetBaseAddress().AbsoluteUri;
		var keycloakClient = new KeycloakClient(baseAddress, KeycloakBuilder.DefaultUsername, KeycloakBuilder.DefaultPassword);

		// When
		var masterRealm = await keycloakClient.GetRealmAsync("master");

		// Then
		Assert.True(masterRealm.Enabled);
	}
}
