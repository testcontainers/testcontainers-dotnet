namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Security.Cryptography.X509Certificates;
  using Docker.DotNet.X509;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Org.BouncyCastle.Crypto;
  using Org.BouncyCastle.Crypto.Parameters;
  using Org.BouncyCastle.OpenSsl;
  using Org.BouncyCastle.Pkcs;
  using Org.BouncyCastle.Security;
  using Org.BouncyCastle.X509;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  [PublicAPI]
  internal sealed class MTlsEndpointAuthenticationProvider : TlsEndpointAuthenticationProvider
  {
    private static readonly X509CertificateParser CertificateParser = new X509CertificateParser();

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

        var keyObject = new PemReader(keyPairStream).ReadObject();

        var certificateEntry = new X509CertificateEntry(certificate);

        var keyParameter = ResolveKeyParameter(keyObject);

        var keyEntry = new AsymmetricKeyEntry(keyParameter);
        store.SetKeyEntry(certificate.SubjectDN + "_key", keyEntry, new[] { certificateEntry });

        using (var certificateStream = new MemoryStream())
        {
          store.Save(certificateStream, password.ToCharArray(), new SecureRandom());
          return new X509Certificate2(Pkcs12Utilities.ConvertToDefiniteLength(certificateStream.ToArray()), password);
        }
      }
    }

    private static AsymmetricKeyParameter ResolveKeyParameter(object keyObject)
    {
      switch (keyObject)
      {
        case AsymmetricCipherKeyPair ackp:
          return ackp.Private;
        case RsaPrivateCrtKeyParameters rpckp:
          return rpckp;
        default:
          throw new ArgumentOutOfRangeException(nameof(keyObject), $"Unsupported asymmetric key entry encountered while trying to resolve key from input object '{keyObject.GetType()}'.");
      }
    }
  }
}
