namespace Testcontainers.Camunda;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class CamundaConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CamundaConfiguration" /> class.
    /// </summary>
    public CamundaConfiguration()
    {
        
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CamundaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>    
    public CamundaConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CamundaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CamundaConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CamundaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CamundaConfiguration(CamundaConfiguration resourceConfiguration)
        : this(new CamundaConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CamundaConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public CamundaConfiguration(CamundaConfiguration oldValue, CamundaConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}