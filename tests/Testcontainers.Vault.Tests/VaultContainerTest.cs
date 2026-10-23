namespace Testcontainers.Vault;

public abstract class VaultContainerTest : IAsyncLifetime
{
    private readonly VaultContainer _vaultContainer;

    private readonly string _rootToken;

    private VaultContainerTest(VaultContainer vaultContainer, string rootToken)
    {
        _vaultContainer = vaultContainer;
        _rootToken = rootToken;
    }

    // # --8<-- [start:UseVaultContainer]
    public async ValueTask InitializeAsync()
    {
        await _vaultContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReadSecretReturnsWrittenSecret()
    {
        // Given
        const string helloWorld = "Hello, World!";

        var path = Guid.NewGuid().ToString("D");

        var authMethod = new TokenAuthMethodInfo(_rootToken);

        var vaultClient = new VaultClient(new VaultClientSettings(_vaultContainer.GetBaseAddress(), authMethod));

        // When
        _ = await vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync(path, new Dictionary<string, object> { { "message", helloWorld } }, mountPoint: "secret")
            .ConfigureAwait(true);

        var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path, mountPoint: "secret")
            .ConfigureAwait(true);

        // Then
        Assert.Equal(helloWorld, secret.Data.Data["message"].ToString());
        Assert.Equal(_vaultContainer.GetBaseAddress(), _vaultContainer.GetConnectionString());
    }
    // # --8<-- [end:UseVaultContainer]

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _vaultContainer.DisposeAsync();
    }

    // # --8<-- [start:CreateVaultContainer]
    [UsedImplicitly]
    public sealed class VaultDefaultConfiguration : VaultContainerTest
    {
        public VaultDefaultConfiguration()
            : base(new VaultBuilder(TestSession.GetImageFromDockerfile()).Build(), VaultBuilder.DefaultRootToken)
        {
        }
    }
    // # --8<-- [end:CreateVaultContainer]

    // # --8<-- [start:InitCommandContainerConfiguration]
    [UsedImplicitly]
    public sealed class VaultInitCommandConfiguration : VaultContainerTest
    {
        private static readonly string RootToken = Guid.NewGuid().ToString("D");

        public VaultInitCommandConfiguration()
            : base(new VaultBuilder(TestSession.GetImageFromDockerfile()).WithRootToken(RootToken).WithInitCommand("secrets enable transit", "write -f transit/keys/orders").Build(), RootToken)
        {
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task ReadEncryptionKeyReturnsCreatedKey()
        {
            // Given
            var authMethod = new TokenAuthMethodInfo(_rootToken);

            var vaultClient = new VaultClient(new VaultClientSettings(_vaultContainer.GetBaseAddress(), authMethod));

            // When
            var encryptionKey = await vaultClient.V1.Secrets.Transit.ReadEncryptionKeyAsync("orders")
                .ConfigureAwait(true);

            // Then
            Assert.Equal("orders", encryptionKey.Data.Name);
        }
    }
    // # --8<-- [end:InitCommandContainerConfiguration]
}