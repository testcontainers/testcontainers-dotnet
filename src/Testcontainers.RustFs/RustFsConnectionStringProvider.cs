namespace Testcontainers.RustFs;

/// <summary>
/// Provides the RustFS connection string.
/// </summary>
internal sealed class RustFsConnectionStringProvider : ContainerConnectionStringProvider<RustFsContainer, RustFsConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}