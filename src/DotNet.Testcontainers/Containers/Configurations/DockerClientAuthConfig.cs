namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Net.Security;
  using System.Security.Cryptography.X509Certificates;
  using Clients;
  using Docker.DotNet;
  using Docker.DotNet.X509;

  public sealed class DockerClientAuthConfig
  {
    private const string DockerTlsVerifyEnvName = "DOCKER_TLS_VERIFY";
    private const string DockerCertPathEnvName = "DOCKER_CERT_PATH";

    private static readonly string[] RequiredCertFiles = {"ca.pem", "cert.pem", "key.pem"};

    private DockerClientAuthConfig(Credentials credentials)
    {
      this.Credentials = credentials;
    }

    public Credentials Credentials { get; }

    public static DockerClientAuthConfig Anonymous() => new DockerClientAuthConfig(new AnonymousCredentials());

    public static DockerClientAuthConfig FromEnv()
    {
      // check if TLS_VERIFY is set and enabled otherwise don't authenticate at all
      var anonymous = new AnonymousCredentials();
      if (!(bool.TryParse(Environment.GetEnvironmentVariable(DockerTlsVerifyEnvName) ?? "", out var tlsEnabled) && tlsEnabled))
      {
        return Anonymous();
      }

      var certPath = Environment.GetEnvironmentVariable(DockerCertPathEnvName) ?? "";

      // if no path is set - it's assumed there are not certs
      if (string.IsNullOrEmpty(certPath))
      {
        return Anonymous();
      }

      return FromCertsDirectory(certPath);
    }

    public static DockerClientAuthConfig FromCertsDirectory(string certPath)
    {
      var allRequireFilesPresent = RequiredCertFiles
        .Select(f => Path.Combine(certPath, f))
        .All(File.Exists);

      if (!allRequireFilesPresent)
      {
        return Anonymous();
      }

      var caCert = PemCertReader.LoadRsaPublicKey(Path.Combine(certPath, "ca.pem"));
      var clientCert = PemCertReader.LoadRsaPublicPrivateKey(Path.Combine(certPath, "cert.pem"), Path.Combine(certPath, "key.pem"));

      var credentials = new CertificateCredentials(clientCert)
      {
        ServerCertificateValidationCallback = CallbackForCustomCaCert(caCert)
      };
      return new DockerClientAuthConfig(credentials);

    }

    /// <summary>
    /// Setup a custom RemoteCertificateValidationCallback to check for the Docker CA cert
    /// Source: https://www.meziantou.net/custom-certificate-validation-in-dotnet.htm
    /// </summary>
    /// <param name="caCert">
    ///   Custom CA cert used to validate the Docker API server cert
    /// </param>
    /// <returns></returns>
    private static RemoteCertificateValidationCallback CallbackForCustomCaCert(X509Certificate2 caCert)
    {
      return (_, _, chain, _) =>
      {
        if (chain.ChainStatus.Any(status => status.Status != X509ChainStatusFlags.UntrustedRoot))
        {
          return false;
        }

        foreach (var element in chain.ChainElements)
        {
          foreach (var status in element.ChainElementStatus)
          {
            if (status.Status == X509ChainStatusFlags.UntrustedRoot)
            {
              if (element.Certificate.Equals(caCert))
              {
                continue;
              }
            }

            return false;
          }
        }

        return true;
      };
    }

  }
}
