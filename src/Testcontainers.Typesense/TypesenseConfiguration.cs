namespace Testcontainers.Typesense;

public class TypesenseConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseConfiguration" /> class.
    /// </summary>
    public TypesenseConfiguration(
      int port = TypesenseBuilder.DefaultPort,
      string apiKey = TypesenseBuilder.DefaultApiKey,
      bool enableCors = TypesenseBuilder.DefaultCors,
      string volume = TypesenseBuilder.DefaultVolume)
    {
        Port = port;
        ApiKey = apiKey;
        EnableCors = enableCors;
        Volume = volume;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TypesenseConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TypesenseConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TypesenseConfiguration(TypesenseConfiguration resourceConfiguration)
        : this(new TypesenseConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypesenseConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public TypesenseConfiguration(TypesenseConfiguration oldValue, TypesenseConfiguration newValue)
        : base(oldValue, newValue)
    {
        ApiKey = BuildConfiguration.Combine(oldValue.ApiKey, newValue.ApiKey);
        Port = BuildConfiguration.Combine(oldValue.Port, newValue.Port);
        EnableCors = BuildConfiguration.Combine(oldValue.EnableCors, newValue.EnableCors);
        Volume = BuildConfiguration.Combine(oldValue.Volume, newValue.Volume);
    }

    public string ApiKey { get; }

    public int Port { get; }

    public bool EnableCors { get; }

    public string Volume { get; }
}
