namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.IO;
  using System.Linq;
  using Docker.DotNet;
  using Docker.DotNet.X509;
  using Services;

  internal static class DockerApiEndpoint
  {
    private const string DockerTlsVerifyEnvName = "DOCKER_TLS_VERIFY";
    private const string DockerCertPathEnvName = "DOCKER_CERT_PATH";

    private static readonly string[] RequiredCertFiles = { "ca.pem", "cert.pem", "key.pem" };

    public static Uri Default => TestcontainersHostService.GetService<IOperatingSystem>().DockerApiEndpoint;

    public static Credentials DefaultCredentials => DetermineCredentialsFromEnv();

    internal static Credentials DetermineCredentialsFromEnv() => DetermineCredentials(
      Environment.GetEnvironmentVariable(DockerTlsVerifyEnvName),
      Environment.GetEnvironmentVariable(DockerCertPathEnvName)
    );

    internal static Credentials DetermineCredentials(string tlsVerify, string certPath)
    {
      // check if TLS_VERIFY is set and enabled otherwise don't authenticate at all
      var anonymous = new AnonymousCredentials();
      if (string.IsNullOrEmpty(tlsVerify) || !(bool.TryParse(tlsVerify, out var tlsEnabled) && tlsEnabled))
      {
        return anonymous;
      }

      // if no path is set - it's assumed there are not certs
      if (string.IsNullOrEmpty(certPath))
      {
        return anonymous;
      }

      var allRequireFilesPresent = RequiredCertFiles
        .Select(f => Path.Combine(certPath, f))
        .All(File.Exists);

      if (!allRequireFilesPresent)
      {
        return anonymous;
      }

      //TODO read certs
      return new CertificateCredentials(null);
    }
  }
}
