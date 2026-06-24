namespace Testcontainers.LocalStack;

/// <summary>
/// Provides the LocalStack connection string.
/// </summary>
internal sealed class LocalStackConnectionStringProvider : ContainerConnectionStringProvider<LocalStackContainer, LocalStackConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}