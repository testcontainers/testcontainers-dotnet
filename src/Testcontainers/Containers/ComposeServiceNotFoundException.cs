namespace DotNet.Testcontainers.Containers
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an exception that is thrown when a Docker Compose service does
  /// not belong to the Docker Compose project, or when the Docker Compose
  /// services have not been started yet.
  /// </summary>
  [PublicAPI]
  public sealed class ComposeServiceNotFoundException : InvalidOperationException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeServiceNotFoundException" /> class.
    /// </summary>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="projectName">The Docker Compose project name.</param>
    public ComposeServiceNotFoundException(string serviceName, string projectName)
      : base($"The Docker Compose service '{serviceName}' was not found in the project '{projectName}'. Make sure the service is part of the Docker Compose file and that the Docker Compose services have been started by calling StartAsync(CancellationToken).")
    {
    }
  }
}
