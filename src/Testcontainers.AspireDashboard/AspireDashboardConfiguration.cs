namespace Testcontainers.AspireDashboard;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class AspireDashboardConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardConfiguration" /> class.
    /// </summary>
    public AspireDashboardConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AspireDashboardConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The container configuration.</param>
    public AspireDashboardConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Aspire dashboard configuration.</param>
    /// <param name="newValue">The new Aspire dashboard configuration.</param>
    public AspireDashboardConfiguration(AspireDashboardConfiguration oldValue, AspireDashboardConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}