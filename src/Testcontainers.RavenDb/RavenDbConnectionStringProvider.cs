namespace Testcontainers.RavenDb;

/// <summary>
/// Provides the RavenDb connection string.
/// </summary>
internal sealed class RavenDbConnectionStringProvider : ContainerConnectionStringProvider<RavenDbContainer, RavenDbConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}