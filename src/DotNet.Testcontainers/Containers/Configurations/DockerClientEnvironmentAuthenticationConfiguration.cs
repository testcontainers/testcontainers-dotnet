namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using System.IO;
  using System.Linq;

  /// <inheritdoc cref="IDockerClientAuthenticationConfiguration" />
  internal sealed class DockerClientEnvironmentAuthenticationConfiguration : IDockerClientAuthenticationConfiguration
  {
    private static readonly string[] RequiredCertificateFiles = { "ca.pem", "cert.pem", "key.pem" };

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerClientEnvironmentAuthenticationConfiguration" /> class.
    /// </summary>
    public DockerClientEnvironmentAuthenticationConfiguration()
      : this(GetDockerHostEnvVariable(), GetDockerCertPathEnvVariable(), GetDockerTlsVerifyEnvVariable())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerClientEnvironmentAuthenticationConfiguration" /> class.
    /// </summary>
    /// <param name="clientEndpoint">The Docker client endpoint.</param>
    /// <param name="certificatesDirectory">The TLS certificates base directory.</param>
    /// <param name="isTlsVerificationEnabled">Is TLS verification enabled.</param>
    public DockerClientEnvironmentAuthenticationConfiguration(Uri clientEndpoint, string certificatesDirectory, bool isTlsVerificationEnabled)
    {
      var hasRequiredCertificates = Directory.Exists(certificatesDirectory) && RequiredCertificateFiles.Length.Equals(
        Directory.EnumerateFiles(certificatesDirectory, "*.*", SearchOption.TopDirectoryOnly)
          .Select(Path.GetFileName)
          .Intersect(RequiredCertificateFiles, StringComparer.OrdinalIgnoreCase)
          .Count());

      this.Endpoint = clientEndpoint;
      this.CertificatesDirectory = certificatesDirectory;
      this.IsTlsVerificationEnabled = isTlsVerificationEnabled && hasRequiredCertificates;
    }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public bool IsApplicable => !Equals(this.Endpoint, null);

    /// <inheritdoc />
    public bool IsTlsVerificationEnabled { get; }

    /// <inheritdoc />
    public string CertificatesDirectory { get; }

    private static Uri GetDockerHostEnvVariable()
    {
      var dockerHost = Environment.GetEnvironmentVariable("DOCKER_HOST");

      if (dockerHost == null)
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
      return new[] { "true", "1", string.Empty }.Any(trueString => trueString.Equals(dockerTlsVerify, StringComparison.OrdinalIgnoreCase));
    }
  }
}
