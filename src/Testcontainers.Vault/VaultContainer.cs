namespace Testcontainers.Vault;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class VaultContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VaultContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public VaultContainer(VaultConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Vault base address.
    /// </summary>
    /// <returns>The Vault base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(VaultBuilder.VaultPort)).ToString();
    }
}