namespace Testcontainers.BigQuery;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class BigQueryConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BigQueryConfiguration" /> class.
    /// </summary>
    /// <param name="config">The BigQuery config.</param>
    public BigQueryConfiguration(string project = null)
    {
        // // Sets the custom builder methods property values.
        Project = project;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigQueryConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public BigQueryConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigQueryConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public BigQueryConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigQueryConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public BigQueryConfiguration(BigQueryConfiguration resourceConfiguration)
        : this(new BigQueryConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigQueryConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public BigQueryConfiguration(BigQueryConfiguration oldValue, BigQueryConfiguration newValue)
        : base(oldValue, newValue)
    {
        // // Create an updated immutable copy of the module configuration.
        Project = BuildConfiguration.Combine(oldValue.Project, newValue.Project);
    }
    
    /// <summary>
    /// Project
    /// </summary>
    public string Project { get; }

    // /// <summary>
    // /// Gets the BigQuery config.
    // /// </summary>
    // public object Config { get; }
}