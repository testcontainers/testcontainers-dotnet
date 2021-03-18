namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Security.Cryptography;
  using System.Security.Cryptography.X509Certificates;
  using System.Text;
  using JetBrains.Annotations;
  using static DockerClientConstants;

  internal sealed class DockerCertificatesDirectory
  {
    private const string RsaOid = "1.2.840.113549.1.1.1";
    private const string DsaOid = "1.2.840.10040.4.1";
    private const string EcdsaOid = "1.2.840.10045.2.1";

    private static readonly string[] RequiredClientCertificateFiles =
    {
      DockerClientCertFileName, DockerClientKeyFileName
    };

    public DockerCertificatesDirectory(string certificatesDirectory)
    {
      var files = Directory.GetFiles(certificatesDirectory, "*.pem", SearchOption.TopDirectoryOnly);
      this.ClientAuthPossible = Directory.Exists(certificatesDirectory) && RequiredClientCertificateFiles.Length.Equals(
        files
          .Select(Path.GetFileName)
          .Intersect(RequiredClientCertificateFiles, StringComparer.CurrentCultureIgnoreCase)
          .Count());

      this.CaCertificatePresent = files.Select(Path.GetFileName).Any(file => string.Equals(file, DockerCaFileName, StringComparison.InvariantCultureIgnoreCase));

      if (this.ClientAuthPossible)
      {
        var publicKeyFilePath = files.First(f => Path.GetFileName(f).Equals(DockerClientCertFileName, StringComparison.InvariantCultureIgnoreCase));
        var privateKeyFilePath = files.First(f => Path.GetFileName(f).Equals(DockerClientKeyFileName, StringComparison.InvariantCultureIgnoreCase));
        this.ClientCertificate = PEMCertWithPrivateKey(publicKeyFilePath, privateKeyFilePath);
      }

      if (this.CaCertificatePresent)
      {
        this.CaCertificate = PEMPublicKey(files.First(f => Path.GetFileName(f).Equals(DockerCaFileName, StringComparison.InvariantCultureIgnoreCase)));
      }
    }

    [CanBeNull]
    public X509Certificate2 CaCertificate { get; }

    [CanBeNull]
    public X509Certificate2 ClientCertificate { get; }

    public bool ClientAuthPossible { get; }

    public bool CaCertificatePresent { get; }

    internal static X509Certificate2 PEMPublicKey(string publicKeyPath)
    {
      var rawPubKey = RawCertContent(publicKeyPath);
      return new X509Certificate2(rawPubKey);
    }

    internal static X509Certificate2 PEMCertWithPrivateKey(string publicKeyPath, string privateKeyPath)
    {
      var cert = PEMPublicKey(publicKeyPath);

      var rawPrivateKey = RawCertContent(privateKeyPath);

      switch (cert.GetKeyAlgorithm())
      {
        case RsaOid:
          var rsa = RSA.Create();
          rsa.ImportRSAPrivateKey(rawPrivateKey, out _);
          return cert.CopyWithPrivateKey(rsa);
        case DsaOid:
          var dsa = DSA.Create();
          dsa.ImportPkcs8PrivateKey(rawPrivateKey, out _);
          return cert.CopyWithPrivateKey(dsa);
        case EcdsaOid:
          var ecdsa = ECDsa.Create();
          ecdsa.ImportECPrivateKey(rawPrivateKey, out _);
          return cert.CopyWithPrivateKey(ecdsa);
        default:
          throw new InvalidOperationException("KeyAlgorithm did not match any expected OID");
      }
    }

    private static byte[] RawCertContent(string filePath)
    {
      var base64EncodedCert = File.ReadAllLines(filePath)
        .Where(line => !line.StartsWith("-----"))
        .Aggregate(new StringBuilder(), (builder, line) => builder.AppendLine(line))
        .ToString();

      return Convert.FromBase64String(base64EncodedCert);
    }
  }
}
