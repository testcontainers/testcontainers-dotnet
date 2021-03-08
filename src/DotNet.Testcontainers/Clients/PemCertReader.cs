namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Security.Cryptography;
  using System.Security.Cryptography.X509Certificates;
  using System.Text;

  internal static class PemCertReader
  {
    internal static X509Certificate2 LoadRsaPublicKey(string pubKeyPath)
    {
      return new X509Certificate2(RawCertContent(pubKeyPath));
    }

    internal static X509Certificate2 LoadRsaPublicPrivateKey(string pubKeyPath, string privKeyPath)
    {
      var cert = new X509Certificate2(RawCertContent(pubKeyPath));
      var rsa = RSA.Create();
      rsa.ImportRSAPrivateKey(RawCertContent(privKeyPath), out _);
      var certWithKey = cert.CopyWithPrivateKey(rsa);
      return certWithKey;
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
