namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using System.IO;
  using System.Linq;
  using Docker.DotNet;

  /// <inheritdoc cref="IDockerClientConfiguration" />
  internal sealed class DockerClientEnvironmentConfiguration : IDockerClientConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerClientEnvironmentConfiguration" /> class.
    /// </summary>
    public DockerClientEnvironmentConfiguration() : this(GetDockerHostEnvVariable(), GetDockerCertPathEnvVariable(), GetDockerTlsVerifyEnvVariable())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerClientEnvironmentConfiguration" /> class.
    /// </summary>
    /// <param name="clientEndpoint">The Docker client endpoint.</param>
    /// <param name="certificatesDirectory">The TLS certificates base directory.</param>
    /// <param name="isTlsVerificationEnabled">Is TLS verification enabled.</param>
    public DockerClientEnvironmentConfiguration(Uri clientEndpoint, string certificatesDirectory, bool isTlsVerificationEnabled)
    {
      this.Endpoint = clientEndpoint;
      var dockerCertificates = new DockerCertificatesDirectory(certificatesDirectory);
      this.Credentials = new TlsCredentials(dockerCertificates.CaCertificate, dockerCertificates.ClientCertificate, isTlsVerificationEnabled, isTlsVerificationEnabled);
    }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public bool IsApplicable => !Equals(this.Endpoint, null);

    /// <inheritdoc />
    public Credentials Credentials { get; }

    private static Uri GetDockerHostEnvVariable()
    {
      var dockerHost = Environment.GetEnvironmentVariable("DOCKER_HOST");

      if (string.IsNullOrEmpty(dockerHost))
      {
        return null;
      }

      try
      {
        return new Uri(dockerHost);
      }
      catch (ArgumentNullException)
      {
        return null;
      }
      catch (UriFormatException)
      {
        return null;
      }
    }

    private static string GetDockerCertPathEnvVariable()
    {
      var dockerCertPath = Environment.GetEnvironmentVariable("DOCKER_CERT_PATH");
      return string.IsNullOrEmpty(dockerCertPath) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker") : dockerCertPath;
    }

    private static bool GetDockerTlsVerifyEnvVariable()
    {
      var dockerTlsVerify = Environment.GetEnvironmentVariable("DOCKER_TLS_VERIFY");
      return new[]
      {
        "true", "1", string.Empty
      }.Any(trueString => trueString.Equals(dockerTlsVerify, StringComparison.OrdinalIgnoreCase));
    }
  }
}
