namespace Testcontainers.MsSql;

/// <summary>
/// Provides the MsSql connection string.
/// </summary>
internal sealed class MsSqlConnectionStringProvider : ContainerConnectionStringProvider<MsSqlContainer, MsSqlConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}