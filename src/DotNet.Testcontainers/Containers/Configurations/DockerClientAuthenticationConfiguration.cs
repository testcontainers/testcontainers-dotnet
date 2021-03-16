namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using System.Linq;
  using DotNet.Testcontainers.Clients;

  /// <inheritdoc cref="IDockerClientAuthenticationConfiguration" />
  internal sealed class DockerClientAuthenticationConfiguration : IDockerClientAuthenticationConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerClientAuthenticationConfiguration" /> class.
    /// </summary>
    public DockerClientAuthenticationConfiguration()
      : this(DockerApiEndpoint.Local)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerClientAuthenticationConfiguration" /> class.
    /// </summary>
    /// <param name="clientEndpoint">The Docker client endpoint.</param>
    public DockerClientAuthenticationConfiguration(Uri clientEndpoint)
    {
      this.Endpoint = clientEndpoint;
    }

    /// <summary>
    /// Gets the default Docker client auth configuration.
    /// </summary>
    public static IDockerClientAuthenticationConfiguration Default { get; }
      = new IDockerClientAuthenticationConfiguration[] { new DockerClientEnvironmentAuthenticationConfiguration(), new DockerClientAuthenticationConfiguration() }
        .First(clientAuthConfig => clientAuthConfig.IsApplicable);

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public bool IsApplicable => !Equals(this.Endpoint, null);

    /// <inheritdoc />
    public bool IsTlsVerificationEnabled => false;

    /// <inheritdoc />
    public string CertificatesDirectory => null;
  }
}
