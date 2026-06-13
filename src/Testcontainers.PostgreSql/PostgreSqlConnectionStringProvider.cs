namespace Testcontainers.PostgreSql;

/// <summary>
/// Provides the PostgreSql connection string.
/// </summary>
internal sealed class PostgreSqlConnectionStringProvider : ContainerConnectionStringProvider<PostgreSqlContainer, PostgreSqlConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}