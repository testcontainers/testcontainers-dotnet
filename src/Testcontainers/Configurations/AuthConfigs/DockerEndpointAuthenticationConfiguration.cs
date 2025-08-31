namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using Docker.DotNet;
  using DotNet.Testcontainers.Clients;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerEndpointAuthenticationConfiguration" />
  [PublicAPI]
  public readonly struct DockerEndpointAuthenticationConfiguration
    : IDockerEndpointAuthenticationConfiguration
  {
    // Since the static `TestcontainersSettings` class holds the detected container
    // runtime information from the auto-discovery mechanism, we can't add a static
    // `NamedPipeConnectionTimeout` property to it because that would create a
    // circular dependency during discovery. To fix this, we either need to split the
    // class or stop exposing the `TestcontainersSettings` properties publicly.
    // Instead, we could rely only on custom configurations via environment variables
    // or the properties file.
    private static readonly TimeSpan NamedPipeConnectionTimeout =
      EnvironmentConfiguration.Instance.GetNamedPipeConnectionTimeout()
      ?? PropertiesFileConfiguration.Instance.GetNamedPipeConnectionTimeout()
      ?? TimeSpan.FromSeconds(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerEndpointAuthenticationConfiguration" /> struct.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <param name="credentials">The Docker API authentication credentials.</param>
    public DockerEndpointAuthenticationConfiguration(Uri endpoint, Credentials credentials = null)
    {
      Endpoint = endpoint;
      Credentials = credentials;
    }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public Credentials Credentials { get; }

    /// <inheritdoc />
    public DockerClientConfiguration GetDockerClientConfiguration(Guid sessionId = default)
    {
      var defaultHttpRequestHeaders = new Dictionary<string, string>();
      defaultHttpRequestHeaders.Add("User-Agent", "tc-dotnet/" + TestcontainersClient.Version);
      defaultHttpRequestHeaders.Add("x-tc-sid", sessionId.ToString("D"));

      return new DockerClientConfiguration(
        Endpoint,
        Credentials,
        namedPipeConnectTimeout: NamedPipeConnectionTimeout,
        defaultHttpRequestHeaders: defaultHttpRequestHeaders
      );
    }
  }
}
