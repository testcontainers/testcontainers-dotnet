namespace Testcontainers.MiniStack;

/// <summary>
/// Provides the MiniStack connection string.
/// </summary>
internal sealed class MiniStackConnectionStringProvider : ContainerConnectionStringProvider<MiniStackContainer, MiniStackConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}
