namespace Testcontainers.Qdrant;

public record PemCertificate(string Certificate, string PrivateKey, string Thumbprint)
{
    public static PemCertificate Create(string commonName)
    {
        using var key = RSA.Create(2048);
        var utcNow = DateTimeOffset.UtcNow;
        var request = new CertificateRequest(
            $"CN={commonName}",
            key,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1)
        {
            CertificateExtensions = { new X509BasicConstraintsExtension(false, false, 0, true) },
        };

        var certificate = request.CreateSelfSigned(utcNow, utcNow.AddYears(1));
        return new PemCertificate(
            certificate.ExportCertificatePem(), 
            certificate.GetRSAPrivateKey().ExportPkcs8PrivateKeyPem(),
            certificate.GetCertHashString(HashAlgorithmName.SHA256));
    }
}