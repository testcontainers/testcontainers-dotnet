namespace Testcontainers.Dapr;


[PublicAPI]
public sealed class DaprConfiguration : ContainerConfiguration
{
    public DaprConfiguration(string appId = null, string logLevel = null, int appPort = 0)
    { 
        AppId = appId;
        LogLevel = logLevel;
        appPort = appPort;
    }
    
    public DaprConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    public DaprConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    public DaprConfiguration(DaprConfiguration resourceConfiguration)
        : this(new DaprConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    public DaprConfiguration(DaprConfiguration oldValue, DaprConfiguration newValue)
        : base(oldValue, newValue)
    {
        AppId = BuildConfiguration.Combine(oldValue.AppId, newValue.AppId);
    }

    public string AppId { get; }
    public string LogLevel { get; }
    public int AppPort { get; set; }
}