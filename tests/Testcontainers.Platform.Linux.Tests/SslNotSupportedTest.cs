namespace Testcontainers.Tests;

public sealed class SslNotSupportedTest
{
    private static readonly string CertificateFilePath = Certificates.Instance.GetFilePath("server", "server.crt");

    private static readonly string CertificateKeyFilePath = Certificates.Instance.GetFilePath("server", "server.key");

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void WithSslThrowsInvalidOperationException()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new ContainerBuilder().WithSsl(CertificateFilePath, CertificateKeyFilePath).Build());
        Assert.Equal("The module does not support SSL.", exception.Message);
    }
}