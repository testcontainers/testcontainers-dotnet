namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Security.Cryptography.X509Certificates;
  using Docker.DotNet.X509;
  using DotNet.Testcontainers.Configurations;
  using Org.BouncyCastle.Crypto;
  using Org.BouncyCastle.OpenSsl;
  using Org.BouncyCastle.Pkcs;
  using Org.BouncyCastle.Security;
  using Org.BouncyCastle.X509;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  internal sealed class MTlsEndpointAuthenticationProvider : TlsEndpointAuthenticationProvider
  {
    private static readonly X509CertificateParser CertificateParser = new X509CertificateParser();

    /// <summary>
    /// Initializes a new instance of the <see cref="MTlsEndpointAuthenticationProvider" /> class.
    /// </summary>
    public MTlsEndpointAuthenticationProvider()
      : this(PropertiesFileConfiguration.Instance, EnvironmentConfiguration.Instance)
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
      return this.TlsEnabled && this.TlsVerifyEnabled && certificatesFiles.Select(fileName => Path.Combine(this.CertificatesDirectoryPath, fileName)).All(File.Exists);
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      var credentials = new CertificateCredentials(this.GetClientCertificate());
      credentials.ServerCertificateValidationCallback = this.ServerCertificateValidationCallback;
      return new DockerEndpointAuthenticationConfiguration(this.DockerEngine, credentials);
    }

    /// <inheritdoc />
    protected override X509Certificate2 GetClientCertificate()
    {
      var clientCertificateFilePath = Path.Combine(this.CertificatesDirectoryPath, ClientCertificateFileName);
      var clientCertificateKeyFilePath = Path.Combine(this.CertificatesDirectoryPath, ClientCertificateKeyFileName);
      return CreateFromPemFile(clientCertificateFilePath, clientCertificateKeyFilePath);
    }

    private static X509Certificate2 CreateFromPemFile(string certPemFilePath, string keyPemFilePath)
    {
      if (!File.Exists(certPemFilePath))
      {
        throw new FileNotFoundException(certPemFilePath);
      }

      if (!File.Exists(keyPemFilePath))
      {
        throw new FileNotFoundException(keyPemFilePath);
      }

      using (var keyPairStream = new StreamReader(keyPemFilePath))
      {
        var store = new Pkcs12StoreBuilder().Build();

        var certificate = CertificateParser.ReadCertificate(File.ReadAllBytes(certPemFilePath));

        var password = Guid.NewGuid().ToString("D");

        var keyPair = (AsymmetricCipherKeyPair)new PemReader(keyPairStream).ReadObject();

        var certificateEntry = new X509CertificateEntry(certificate);

        var keyEntry = new AsymmetricKeyEntry(keyPair.Private);
        store.SetKeyEntry(certificate.SubjectDN + "_key", keyEntry, new[] { certificateEntry });

        using (var certificateStream = new MemoryStream())
        {
          store.Save(certificateStream, password.ToCharArray(), new SecureRandom());
          return new X509Certificate2(Pkcs12Utilities.ConvertToDefiniteLength(certificateStream.ToArray()), password);
        }
      }
    }
  }
}
