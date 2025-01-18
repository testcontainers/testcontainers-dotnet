namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Security.Cryptography.X509Certificates;
  using Docker.DotNet.X509;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  [PublicAPI]
  internal sealed class MTlsEndpointAuthenticationProvider : TlsEndpointAuthenticationProvider
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MTlsEndpointAuthenticationProvider" /> class.
    /// </summary>
    public MTlsEndpointAuthenticationProvider()
      : this(EnvironmentConfiguration.Instance, PropertiesFileConfiguration.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MTlsEndpointAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="customConfigurations">A list of custom configurations.</param>
    public MTlsEndpointAuthenticationProvider(params ICustomConfiguration[] customConfigurations)
      : base(customConfigurations)
    {
    }

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      var certificatesFiles = new[] { ClientCertificateFileName, ClientCertificateKeyFileName };
      return TlsEnabled && TlsVerifyEnabled && certificatesFiles.Select(fileName => Path.Combine(CertificatesDirectoryPath, fileName)).All(File.Exists);
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      var credentials = new CertificateCredentials(GetClientCertificate());
      credentials.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
      return new DockerEndpointAuthenticationConfiguration(DockerEngine, credentials);
    }

    /// <inheritdoc />
    protected override X509Certificate2 GetClientCertificate()
    {
      var clientCertificateFilePath = Path.Combine(CertificatesDirectoryPath, ClientCertificateFileName);
      var clientCertificateKeyFilePath = Path.Combine(CertificatesDirectoryPath, ClientCertificateKeyFileName);

      // The certificate must be exported to PFX on Windows to avoid "No credentials are available in the security package":
      // https://stackoverflow.com/questions/72096812/loading-x509certificate2-from-pem-file-results-in-no-credentials-are-available/72101855#72101855.
#if NETSTANDARD
      return Polyfills.X509Certificate2.CreateFromPemFile(clientCertificateFilePath, clientCertificateKeyFilePath);
#elif NET9_0_OR_GREATER
      var certificate = X509Certificate2.CreateFromPemFile(clientCertificateFilePath, clientCertificateKeyFilePath);
      return OperatingSystem.IsWindows() ? X509CertificateLoader.LoadPkcs12(certificate.Export(X509ContentType.Pfx), null) : certificate;
#elif NET6_0_OR_GREATER
      var certificate = X509Certificate2.CreateFromPemFile(clientCertificateFilePath, clientCertificateKeyFilePath);
      return OperatingSystem.IsWindows() ? new X509Certificate2(certificate.Export(X509ContentType.Pfx)) : certificate;
#endif
    }
  }
}
