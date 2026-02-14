namespace Testcontainers.Temporalio;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class TemporalConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalConfiguration" /> class.
    /// </summary>
    /// <param name="namespaces">The namespaces to pre-register at startup.</param>
    /// <param name="searchAttributes">The search attributes to register.</param>
    /// <param name="dynamicConfigValues">The dynamic configuration values.</param>
    /// <param name="dbFilename">The path to the database file for persistent state.</param>
    public TemporalConfiguration(
        IEnumerable<string> namespaces = null,
        IEnumerable<string> searchAttributes = null,
        IEnumerable<string> dynamicConfigValues = null,
        string dbFilename = null)
    {
        Namespaces = namespaces;
        SearchAttributes = searchAttributes;
        DynamicConfigValues = dynamicConfigValues;
        DbFilename = dbFilename;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TemporalConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TemporalConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TemporalConfiguration(TemporalConfiguration resourceConfiguration)
        : this(new TemporalConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public TemporalConfiguration(TemporalConfiguration oldValue, TemporalConfiguration newValue)
        : base(oldValue, newValue)
    {
        Namespaces = BuildConfiguration.Combine(oldValue.Namespaces, newValue.Namespaces);
        SearchAttributes = BuildConfiguration.Combine(oldValue.SearchAttributes, newValue.SearchAttributes);
        DynamicConfigValues = BuildConfiguration.Combine(oldValue.DynamicConfigValues, newValue.DynamicConfigValues);
        DbFilename = BuildConfiguration.Combine(oldValue.DbFilename, newValue.DbFilename);
    }

    /// <summary>
    /// Gets the namespaces to pre-register at startup.
    /// The "default" namespace is always registered regardless of this setting.
    /// </summary>
    public IEnumerable<string> Namespaces { get; }

    /// <summary>
    /// Gets the search attributes to register in <c>KEY=TYPE</c> format.
    /// Type is one of: Text, Keyword, Int, Double, Bool, Datetime, KeywordList.
    /// </summary>
    public IEnumerable<string> SearchAttributes { get; }

    /// <summary>
    /// Gets the dynamic configuration values in <c>KEY=JSON_VALUE</c> format.
    /// </summary>
    public IEnumerable<string> DynamicConfigValues { get; }

    /// <summary>
    /// Gets the path to the database file for persistent Temporal state inside the container.
    /// </summary>
    public string DbFilename { get; }
}
