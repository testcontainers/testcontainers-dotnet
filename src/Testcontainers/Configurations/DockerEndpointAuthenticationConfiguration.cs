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
    /// <param name="credentials">The Docker API authentication credentials.</param>
    public DockerEndpointAuthenticationConfiguration(Uri endpoint, Credentials credentials = null)
    {
      this.Credentials = credentials;
      this.Endpoint = endpoint;
    }

    /// <inheritdoc />
    public Credentials Credentials { get; }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public DockerClientConfiguration GetDockerClientConfiguration(Guid sessionId = default)
    {
      var defaultHttpRequestHeaders = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> { { "x-tc-sid", sessionId.ToString("D") } });
      return new DockerClientConfiguration(this.Endpoint, this.Credentials, defaultHttpRequestHeaders: defaultHttpRequestHeaders);
    }
  }
}
