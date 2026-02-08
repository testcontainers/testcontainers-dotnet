namespace Testcontainers.CockroachDb;

/// <summary>
/// Provides the CockroachDb connection string.
/// </summary>
internal sealed class CockroachDbConnectionStringProvider : ContainerConnectionStringProvider<CockroachDbContainer, CockroachDbConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}