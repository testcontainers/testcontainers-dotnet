namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Net.Security;
  using System.Security.Cryptography.X509Certificates;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  [PublicAPI]
  internal class TlsEndpointAuthenticationProvider : DockerEndpointAuthenticationProvider
  {
    protected const string CaCertificateFileName = "ca.pem";

    protected const string ClientCertificateFileName = "cert.pem";

    protected const string ClientCertificateKeyFileName = "key.pem";

    /// <summary>
    /// Initializes a new instance of the <see cref="TlsEndpointAuthenticationProvider" /> class.
    /// </summary>
    public TlsEndpointAuthenticationProvider()
      : this(EnvironmentConfiguration.Instance, PropertiesFileConfiguration.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TlsEndpointAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="customConfigurations">A list of custom configurations.</param>
    public TlsEndpointAuthenticationProvider(params ICustomConfiguration[] customConfigurations)
      : this(customConfigurations
        .OrderByDescending(item => item.GetDockerTlsVerify())
        .ThenByDescending(item => item.GetDockerTls())
        .DefaultIfEmpty(new PropertiesFileConfiguration(Array.Empty<string>()))
        .First())
    {
    }

    private TlsEndpointAuthenticationProvider(ICustomConfiguration customConfiguration)
    {
      TlsEnabled = customConfiguration.GetDockerTls() || customConfiguration.GetDockerTlsVerify();
      TlsVerifyEnabled = customConfiguration.GetDockerTlsVerify();
      CertificatesDirectoryPath = customConfiguration.GetDockerCertPath() ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker");
      DockerEngine = customConfiguration.GetDockerHost() ?? new Uri("tcp://localhost:2376");
    }

    protected bool TlsEnabled { get; }

    protected bool TlsVerifyEnabled { get; }

    protected string CertificatesDirectoryPath { get; }

    protected Uri DockerEngine { get; }

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      return TlsEnabled;
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      var credentials = new TlsCredentials();
      credentials.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
      return new DockerEndpointAuthenticationConfiguration(DockerEngine, credentials);
    }

    /// <summary>
    /// Gets the root certificate authority (CA).
    /// </summary>
    /// <returns>The root certificate authority (CA).</returns>
    protected virtual X509Certificate2 GetCaCertificate()
    {
      return new X509Certificate2(Path.Combine(CertificatesDirectoryPath, CaCertificateFileName));
    }

    /// <summary>
    /// Gets the client certificate.
    /// </summary>
    /// <returns>The client certificate.</returns>
    protected virtual X509Certificate2 GetClientCertificate()
    {
      return null;
    }

    /// <inheritdoc cref="ServicePointManager.ServerCertificateValidationCallback" />
    protected virtual bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
      switch (sslPolicyErrors)
      {
        case SslPolicyErrors.None:
          return true;
        case SslPolicyErrors.RemoteCertificateNameMismatch:
        case SslPolicyErrors.RemoteCertificateNotAvailable:
          return false;
        case SslPolicyErrors.RemoteCertificateChainErrors:
        default:
          using (var caCertificate = GetCaCertificate())
          {
            var validationChain = new X509Chain();
            validationChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            validationChain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            validationChain.ChainPolicy.ExtraStore.Add(caCertificate);
            validationChain.ChainPolicy.ExtraStore.AddRange(chain.ChainElements.OfType<X509ChainElement>().Select(element => element.Certificate).ToArray());
            var isVerified = validationChain.Build(new X509Certificate2(certificate));
            var isSignedByExpectedRoot = validationChain.ChainElements[validationChain.ChainElements.Count - 1].Certificate.RawData.SequenceEqual(caCertificate.RawData);
            return isVerified && isSignedByExpectedRoot;
          }
      }
    }
  }
}
