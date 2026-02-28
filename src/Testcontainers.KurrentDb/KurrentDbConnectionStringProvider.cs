namespace Testcontainers.KurrentDb;

/// <summary>
/// Provides the KurrentDb connection string.
/// </summary>
internal sealed class KurrentDbConnectionStringProvider : ContainerConnectionStringProvider<KurrentDbContainer, KurrentDbConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}