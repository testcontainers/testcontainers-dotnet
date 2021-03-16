namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using System.Linq;
  using Docker.DotNet;
  using DotNet.Testcontainers.Clients;

  /// <inheritdoc cref="IDockerClientConfiguration" />
  internal sealed class DockerClientConfiguration : IDockerClientConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerClientConfiguration" /> class.
    /// </summary>
    public DockerClientConfiguration()
      : this(DockerApiEndpoint.Local)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerClientConfiguration" /> class.
    /// </summary>
    /// <param name="clientEndpoint">The Docker client endpoint.</param>
    public DockerClientConfiguration(Uri clientEndpoint)
    {
      this.Endpoint = clientEndpoint;
    }

    /// <summary>
    /// Gets the default Docker client auth configuration.
    /// </summary>
    public static IDockerClientConfiguration Default { get; }
      = new IDockerClientConfiguration[] { new DockerClientEnvironmentConfiguration(), new DockerClientConfiguration() }
        .First(clientAuthConfig => clientAuthConfig.IsApplicable);

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public bool IsApplicable => !Equals(this.Endpoint, null);

    /// <inheritdoc />
    public Credentials Credentials { get; }
  }
}
