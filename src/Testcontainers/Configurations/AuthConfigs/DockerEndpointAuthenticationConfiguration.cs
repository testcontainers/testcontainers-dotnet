namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using Docker.DotNet;
  using DotNet.Testcontainers.Clients;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerEndpointAuthenticationConfiguration" />
  [PublicAPI]
  public readonly struct DockerEndpointAuthenticationConfiguration : IDockerEndpointAuthenticationConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerEndpointAuthenticationConfiguration" /> struct.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <param name="credentials">The Docker API authentication credentials.</param>
    public DockerEndpointAuthenticationConfiguration(Uri endpoint, Credentials credentials = null)
    {
      Credentials = credentials;
      Endpoint = endpoint;
    }

    /// <inheritdoc />
    public Credentials Credentials { get; }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public DockerClientConfiguration GetDockerClientConfiguration(Guid sessionId = default)
    {
      var defaultHttpRequestHeaders = new Dictionary<string, string>();
      defaultHttpRequestHeaders.Add("User-Agent", "tc-dotnet/" + TestcontainersClient.Version);
      defaultHttpRequestHeaders.Add("x-tc-sid", sessionId.ToString("D"));
      return new DockerClientConfiguration(Endpoint, Credentials, defaultHttpRequestHeaders: defaultHttpRequestHeaders);
    }
  }
}
