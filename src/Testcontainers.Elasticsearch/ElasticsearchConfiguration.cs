namespace Testcontainers.Elasticsearch;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class ElasticsearchConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConfiguration" /> class.
    /// </summary>
    /// <param name="config">The Elasticsearch config.</param>
    public ElasticsearchConfiguration(object config = null)
    {
        // // Sets the custom builder methods property values.
        // Config = config;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ElasticsearchConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ElasticsearchConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ElasticsearchConfiguration(ElasticsearchConfiguration resourceConfiguration)
        : this(new ElasticsearchConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ElasticsearchConfiguration(ElasticsearchConfiguration oldValue, ElasticsearchConfiguration newValue)
        : base(oldValue, newValue)
    {
        // // Create an updated immutable copy of the module configuration.
        // Config = BuildConfiguration.Combine(oldValue.Config, newValue.Config);
    }

    // /// <summary>
    // /// Gets the Elasticsearch config.
    // /// </summary>
    // public object Config { get; }
}