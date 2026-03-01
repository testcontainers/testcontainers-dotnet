namespace Testcontainers.Papercut;

/// <summary>
/// Provides the Papercut connection string.
/// </summary>
internal sealed class PapercutConnectionStringProvider : ContainerConnectionStringProvider<PapercutContainer, PapercutConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBaseAddress();
    }
}