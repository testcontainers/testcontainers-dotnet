namespace DotNet.Testcontainers.Containers
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an exception that is thrown when a Docker Compose service port is
  /// not exposed.
  /// </summary>
  [PublicAPI]
  public sealed class ComposeServiceNotExposedException : InvalidOperationException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeServiceNotExposedException" /> class.
    /// </summary>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="servicePort">The Docker Compose service port.</param>
    public ComposeServiceNotExposedException(string serviceName, ushort servicePort)
      : base($"The port {servicePort} of the Docker Compose service '{serviceName}' is not exposed. Expose the service port using the builder method WithExposedService(string, ushort).")
    {
    }
  }
}
