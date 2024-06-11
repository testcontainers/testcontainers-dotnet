namespace Testcontainers.AspireDashboard;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class AspireDashboardConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardConfiguration" /> class.
    /// </summary>
    public AspireDashboardConfiguration(bool allowAnonymous = false, bool allowUnsecuredTransport = false)
    {
        AllowAnonymous = allowAnonymous;
        AllowUnsecuredTransport = allowUnsecuredTransport;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AspireDashboardConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AspireDashboardConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AspireDashboardConfiguration(AspireDashboardConfiguration resourceConfiguration)
        : this(new AspireDashboardConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public AspireDashboardConfiguration(AspireDashboardConfiguration oldValue, AspireDashboardConfiguration newValue)
        : base(oldValue, newValue)
    {
    }

    /// <summary>
    /// Gets AllowAnonymous mode.
    /// </summary>
    public bool AllowAnonymous { get; }

    /// <summary>
    /// Gets AllowUnsecuredTransport mode.
    /// </summary>
    public bool AllowUnsecuredTransport { get; }
}