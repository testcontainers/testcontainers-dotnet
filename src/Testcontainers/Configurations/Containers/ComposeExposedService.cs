namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// An exposed Docker Compose service port.
  /// </summary>
  [PublicAPI]
  public sealed class ComposeExposedService
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeExposedService" /> class.
    /// </summary>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="instance">The number of the container within the Docker Compose service.</param>
    /// <param name="port">The Docker Compose service port.</param>
    public ComposeExposedService(string serviceName, ushort instance, ushort port)
    {
      ServiceName = serviceName;
      Instance = instance;
      Port = port;
    }

    /// <summary>
    /// Gets the Docker Compose service name.
    /// </summary>
    public string ServiceName { get; }

    /// <summary>
    /// Gets the number of the container within the Docker Compose service.
    /// </summary>
    public ushort Instance { get; }

    /// <summary>
    /// Gets the Docker Compose service port.
    /// </summary>
    public ushort Port { get; }
  }
}
