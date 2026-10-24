namespace Testcontainers.DuckDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class DuckDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuckDbConfiguration" /> class.
    /// </summary>
    /// <param name="database">The DuckDB database file path.</param>
    public DuckDbConfiguration(
        string database = null)
    {
        Database = database;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuckDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DuckDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuckDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DuckDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuckDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DuckDbConfiguration(DuckDbConfiguration resourceConfiguration)
        : this(new DuckDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuckDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public DuckDbConfiguration(DuckDbConfiguration oldValue, DuckDbConfiguration newValue)
        : base(oldValue, newValue)
    {
        Database = BuildConfiguration.Combine(oldValue.Database, newValue.Database);
    }

    /// <summary>
    /// Gets the DuckDB database file path.
    /// </summary>
    public string Database { get; }
}
