namespace Testcontainers.Vault;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class VaultContainer : DockerContainer
{
    private readonly VaultConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="VaultContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public VaultContainer(VaultConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Vault base address.
    /// </summary>
    /// <returns>The Vault base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(VaultBuilder.VaultPort)).ToString();
    }

    /// <summary>
    /// Gets the Vault root token.
    /// </summary>
    /// <returns>The Vault root token.</returns>
    public string GetRootToken()
    {
        return _configuration.RootToken;
    }
}
