namespace Testcontainers.LowkeyVault;

/// <summary>
/// Provides the Lowkey Vault connection string.
/// </summary>
internal sealed class LowkeyVaultConnectionStringProvider : ContainerConnectionStringProvider<LowkeyVaultContainer, LowkeyVaultConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBaseAddress();
    }
}