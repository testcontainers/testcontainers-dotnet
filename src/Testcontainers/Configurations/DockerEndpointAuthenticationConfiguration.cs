namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
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
    public DockerClientConfiguration GetDockerClientConfiguration(Guid sessionId = default)
    {
      var defaultHttpRequestHeaders = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> { { "x-tc-sid", sessionId.ToString("D") } });
      return new DockerClientConfiguration(this.Endpoint, defaultHttpRequestHeaders: defaultHttpRequestHeaders);
    }
  }
}
