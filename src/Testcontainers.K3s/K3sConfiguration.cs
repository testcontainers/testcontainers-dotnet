namespace Testcontainers.K3s;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class K3sConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="K3sConfiguration" /> class.
    /// </summary>
    public K3sConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public K3sConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public K3sConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public K3sConfiguration(K3sConfiguration resourceConfiguration)
        : this(new K3sConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public K3sConfiguration(K3sConfiguration oldValue, K3sConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}