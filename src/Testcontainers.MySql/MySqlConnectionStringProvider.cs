namespace Testcontainers.MySql;

/// <summary>
/// Provides the MySql connection string.
/// </summary>
internal sealed class MySqlConnectionStringProvider : ContainerConnectionStringProvider<MySqlContainer, MySqlConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}