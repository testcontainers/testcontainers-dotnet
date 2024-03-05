#if NETSTANDARD
using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace DotNet.Testcontainers
{
  internal static class Polyfill
  {
    public static class X509Certificate2
    {
      private static readonly X509CertificateParser CertificateParser = new X509CertificateParser();

      public static System.Security.Cryptography.X509Certificates.X509Certificate2 CreateFromPemFile(string certPemFilePath, string keyPemFilePath)
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
            return new System.Security.Cryptography.X509Certificates.X509Certificate2(Pkcs12Utilities.ConvertToDefiniteLength(certificateStream.ToArray()), password);
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
}
#endif
