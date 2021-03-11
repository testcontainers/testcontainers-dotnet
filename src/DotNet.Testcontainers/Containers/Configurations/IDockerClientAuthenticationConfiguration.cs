namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// TODO: Add comment.
  /// </summary>
  public interface IDockerClientAuthenticationConfiguration
  {
    /// <summary>
    /// Gets the Docker API endpoint.
    /// </summary>
    [NotNull]
    Uri Endpoint { get; }
  }
}
