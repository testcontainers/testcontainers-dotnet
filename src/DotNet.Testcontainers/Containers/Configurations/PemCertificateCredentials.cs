namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Net.Security;
  using System.Security.Cryptography;
  using System.Security.Cryptography.X509Certificates;
  using System.Text;
  using Docker.DotNet.X509;

  public class PemCertificateCredentials : CertificateCredentials
  {
    private const string RsaOid = "1.2.840.113549.1.1.1";
    private const string DsaOid = "1.2.840.10040.4.1";
    private const string EcdsaOid = "1.2.840.10045.2.1";

    private static readonly RemoteCertificateValidationCallback Ignore = (_, _, _, _) => true;

    public PemCertificateCredentials(DockerCertDir certificatesDirectory, bool tlsVerify) : base(ReadCertFromCertDirectory(certificatesDirectory))
    {
      this.ServerCertificateValidationCallback = tlsVerify ? SetupServerCertificateValidationCallback(certificatesDirectory) : Ignore;
    }

    private static RemoteCertificateValidationCallback SetupServerCertificateValidationCallback(DockerCertDir certificatesDirectory)
    {
      var rawCaFile = RawCertContent(certificatesDirectory.CaFilePath);
      var caCert = new X509Certificate2(rawCaFile);
      var caCertHash = caCert.GetCertHashString();
      if (string.IsNullOrEmpty(caCertHash))
      {
        throw new InvalidOperationException("CA cert hash is empty - verification not possible");
      }

      return (_, _, chain, errors) =>
      {
        if (errors != SslPolicyErrors.None)
        {
          return false;
        }

        foreach (var element in chain.ChainElements)
        {
          if (caCertHash.Equals(element.Certificate.GetCertHashString()))
          {
            return true;
          }
        }

        return false;
      };
    }

    private static X509Certificate2 ReadCertFromCertDirectory(DockerCertDir certDirectory)
    {
      var rawPubKey = RawCertContent(certDirectory.PublicKeyFilePath);
      var rawPrivKey = RawCertContent(certDirectory.PrivateKeyFilePath);

      var cert = new X509Certificate2(rawPubKey);

      switch (cert.GetKeyAlgorithm())
      {
        case RsaOid:
          var rsa = RSA.Create();
          rsa.ImportRSAPrivateKey(rawPrivKey, out _);
          return cert.CopyWithPrivateKey(rsa);
        case DsaOid:
          var dsa = DSA.Create();
          dsa.ImportPkcs8PrivateKey(rawPrivKey, out _);
          return cert.CopyWithPrivateKey(dsa);
        case EcdsaOid:
          var ecdsa = ECDsa.Create();
          ecdsa.ImportECPrivateKey(rawPrivKey, out _);
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
