namespace Testcontainers.AspireDashboard;

/// <summary>
/// Provides the Aspire dashboard connection string.
/// </summary>
internal sealed class AspireDashboardConnectionStringProvider : ContainerConnectionStringProvider<AspireDashboardContainer, AspireDashboardConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}