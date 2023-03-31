namespace Testcontainers.Dapr;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class DaprConfiguration : ContainerConfiguration
{
  
    public DaprConfiguration(
        string appId = null,
        int appPort = 0,
        int daprHttpPort = 0,
        int daprGrpcPort = 0,
        string logLevel = null)
    {
        AppId = appId;
        AppPort = appPort;
        DaprHttpPort = daprHttpPort;
        DaprGrpcPort = daprGrpcPort;
        LogLevel = logLevel;
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
        AppPort = BuildConfiguration.Combine(oldValue.AppPort, newValue.AppPort);
        DaprHttpPort = BuildConfiguration.Combine(oldValue.DaprHttpPort, newValue.DaprHttpPort);
        DaprGrpcPort = BuildConfiguration.Combine(oldValue.DaprGrpcPort, newValue.DaprGrpcPort);
        LogLevel = BuildConfiguration.Combine(oldValue.LogLevel, newValue.LogLevel);
    }

    public string AppId { get; }

    public int AppPort { get; }

    public int DaprHttpPort { get; }

    public int DaprGrpcPort { get; set; }

    public string LogLevel { get; set; }
}