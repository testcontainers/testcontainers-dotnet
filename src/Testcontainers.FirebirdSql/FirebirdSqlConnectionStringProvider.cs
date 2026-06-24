namespace Testcontainers.FirebirdSql;

/// <summary>
/// Provides the FirebirdSql connection string.
/// </summary>
internal sealed class FirebirdSqlConnectionStringProvider : ContainerConnectionStringProvider<FirebirdSqlContainer, FirebirdSqlConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}