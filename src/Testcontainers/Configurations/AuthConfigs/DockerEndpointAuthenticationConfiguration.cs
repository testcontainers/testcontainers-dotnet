namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using Docker.DotNet;
  using Docker.DotNet.Handler.Abstractions;
  using Docker.DotNet.NPipe;
  using DotNet.Testcontainers.Clients;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerEndpointAuthenticationConfiguration" />
  [PublicAPI]
  public readonly struct DockerEndpointAuthenticationConfiguration : IDockerEndpointAuthenticationConfiguration
  {
    // https://github.com/moby/moby/releases/tag/docker-v29.0.0.
    private static readonly Version DockerEngineApi = EnvironmentConfiguration.Instance.GetDockerApiVersion() ?? PropertiesFileConfiguration.Instance.GetDockerApiVersion() ?? new Version(1, 44);

    // Since the static `TestcontainersSettings` class holds the detected container
    // runtime information from the auto-discovery mechanism, we can't add a static
    // `NPipeConnectTimeout` property to it because that would create a
    // circular dependency during discovery. To fix this, we either need to split the
    // class or stop exposing the `TestcontainersSettings` properties publicly.
    // Instead, we could rely only on custom configurations via environment variables
    // or the properties file.
    private static readonly TimeSpan NPipeConnectTimeout = EnvironmentConfiguration.Instance.GetNamedPipeConnectionTimeout() ?? PropertiesFileConfiguration.Instance.GetNamedPipeConnectionTimeout() ?? TimeSpan.FromSeconds(10);

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerEndpointAuthenticationConfiguration" /> struct.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <param name="authProvider">The Docker API authentication provider.</param>
    public DockerEndpointAuthenticationConfiguration(Uri endpoint, IAuthProvider authProvider = null)
    {
      Endpoint = endpoint;
      AuthProvider = authProvider;
    }

    /// <inheritdoc />
    public Version Version => DockerEngineApi;

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public IAuthProvider AuthProvider { get; }

    /// <inheritdoc />
    public DockerClientBuilder GetDockerClientBuilder(Guid sessionId = default)
    {
      var headers = new Dictionary<string, string>();
      headers.Add("User-Agent", "tc-dotnet/" + TestcontainersClient.Version);
      headers.Add("x-tc-sid", sessionId.ToString("D"));

      var dockerClientBuilder = new DockerClientBuilder()
        .WithApiVersion(Version)
        .WithEndpoint(Endpoint)
        .WithAuthProvider(AuthProvider)
        .WithHeaders(headers);

      if ("npipe".Equals(Endpoint.Scheme, StringComparison.OrdinalIgnoreCase))
      {
        var transportOptions = new NPipeTransportOptions
        {
          ConnectTimeout = NPipeConnectTimeout,
        };

        dockerClientBuilder = dockerClientBuilder
          .WithTransportOptions(transportOptions);
      }

      return dockerClientBuilder;
    }
  }
}
