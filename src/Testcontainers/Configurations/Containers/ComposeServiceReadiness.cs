namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using JetBrains.Annotations;

  /// <summary>
  /// The readiness of a Docker Compose service.
  /// </summary>
  [PublicAPI]
  public sealed class ComposeServiceReadiness
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeServiceReadiness" /> class.
    /// </summary>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="instance">The number of the container within the Docker Compose service.</param>
    /// <param name="waitStrategies">The wait strategies that indicate the readiness of the service.</param>
    public ComposeServiceReadiness(string serviceName, ushort instance, IEnumerable<WaitStrategy> waitStrategies)
    {
      ServiceName = serviceName;
      Instance = instance;
      WaitStrategies = waitStrategies;
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
    /// Gets the wait strategies that indicate the readiness of the service.
    /// </summary>
    public IEnumerable<WaitStrategy> WaitStrategies { get; }
  }
}
