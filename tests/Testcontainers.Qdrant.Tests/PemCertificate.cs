namespace Testcontainers.Qdrant;

public sealed class PemCertificate
{
    static PemCertificate()
    {
    }

    private PemCertificate(string commonName)
    {
        using var rsa = RSA.Create(2048);

        var subjectName = new X500DistinguishedName($"CN={commonName}");

        var request = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        using var certificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

        CommonName = commonName;
        Thumbprint = certificate.GetCertHashString(HashAlgorithmName.SHA256);
        Certificate = certificate.ExportCertificatePem();
        CertificateKey = rsa.ExportPkcs8PrivateKeyPem();
    }

    public static PemCertificate Instance { get; }
        = new PemCertificate("localhost");

    public string CommonName { get; }

    public string Thumbprint { get; }

    public string Certificate { get; }

    public string CertificateKey { get; }
}