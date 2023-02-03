namespace Testcontainers.MsSql;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MsSqlConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlConfiguration" /> class.
    /// </summary>
    /// <param name="config">The MsSql config.</param>
    public MsSqlConfiguration(object config = null)
    {
        // // Sets the custom builder methods property values.
        // Config = config;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MsSqlConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MsSqlConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MsSqlConfiguration(MsSqlConfiguration resourceConfiguration)
        : this(new MsSqlConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MsSqlConfiguration(MsSqlConfiguration oldValue, MsSqlConfiguration newValue)
        : base(oldValue, newValue)
    {
        // // Create an updated immutable copy of the module configuration.
        // Config = BuildConfiguration.Combine(oldValue.Config, newValue.Config);
    }

    // /// <summary>
    // /// Gets the MsSql config.
    // /// </summary>
    // public object Config { get; }
}