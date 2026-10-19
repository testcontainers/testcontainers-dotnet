namespace Testcontainers.Vault;

/// <summary>
/// Provides the Vault connection string.
/// </summary>
internal sealed class VaultConnectionStringProvider : ContainerConnectionStringProvider<VaultContainer, VaultConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBaseAddress();
    }
}
