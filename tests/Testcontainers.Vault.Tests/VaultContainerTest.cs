namespace Testcontainers.Vault;

public sealed class VaultContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseVaultContainer]
    private readonly VaultContainer _vaultContainer = new VaultBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _vaultContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _vaultContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReadSecretReturnsWrittenSecret()
    {
        // Given
        const string password = "Hello, World!";

        var path = Guid.NewGuid().ToString("D");

        var authMethod = new TokenAuthMethodInfo(_vaultContainer.GetRootToken());

        var vaultClient = new VaultClient(new VaultClientSettings(_vaultContainer.GetBaseAddress(), authMethod));

        // When
        _ = await vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync(path, new Dictionary<string, object> { { "password", password } }, mountPoint: "secret")
            .ConfigureAwait(true);

        var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path, mountPoint: "secret")
            .ConfigureAwait(true);

        // Then
        Assert.Equal(password, secret.Data.Data["password"].ToString());
        Assert.Equal(VaultBuilder.DefaultRootToken, _vaultContainer.GetRootToken());
        Assert.Equal(_vaultContainer.GetBaseAddress(), _vaultContainer.GetConnectionString());
    }
    // # --8<-- [end:UseVaultContainer]
}
