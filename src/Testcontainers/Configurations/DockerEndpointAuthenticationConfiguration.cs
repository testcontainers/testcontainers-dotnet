namespace DotNet.Testcontainers.Configurations
{
  using System;
  using Docker.DotNet;

  /// <inheritdoc cref="IDockerEndpointAuthenticationConfiguration" />
  public readonly struct DockerEndpointAuthenticationConfiguration : IDockerEndpointAuthenticationConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerEndpointAuthenticationConfiguration" /> struct.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    public DockerEndpointAuthenticationConfiguration(Uri endpoint)
    {
      this.Endpoint = endpoint;
    }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public DockerClientConfiguration GetDockerClientConfiguration()
    {
      return new DockerClientConfiguration(this.Endpoint);
    }
  }
}
