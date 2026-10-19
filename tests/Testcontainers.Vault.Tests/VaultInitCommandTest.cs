namespace Testcontainers.Vault;

public sealed class VaultInitCommandTest : IAsyncLifetime
{
    // # --8<-- [start:UseVaultInitCommands]
    private readonly VaultContainer _vaultContainer = new VaultBuilder(TestSession.GetImageFromDockerfile())
        .WithRootToken("vault-root-token")
        .WithInitCommand("secrets enable transit", "write -f transit/keys/orders")
        .WithInitCommand("kv put secret/orders-api password=s3cr3t")
        .Build();
    // # --8<-- [end:UseVaultInitCommands]

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
    public async Task InitCommandsRunAfterStart()
    {
        // Given
        var authMethod = new TokenAuthMethodInfo(_vaultContainer.GetRootToken());

        var vaultClient = new VaultClient(new VaultClientSettings(_vaultContainer.GetBaseAddress(), authMethod));

        // When
        var encryptionKey = await vaultClient.V1.Secrets.Transit.ReadEncryptionKeyAsync("orders")
            .ConfigureAwait(true);

        var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("orders-api", mountPoint: "secret")
            .ConfigureAwait(true);

        // Then
        Assert.Equal("vault-root-token", _vaultContainer.GetRootToken());
        Assert.Equal("orders", encryptionKey.Data.Name);
        Assert.Equal("s3cr3t", secret.Data.Data["password"].ToString());
    }
}
