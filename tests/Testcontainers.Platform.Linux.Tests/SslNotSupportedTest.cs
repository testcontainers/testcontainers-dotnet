namespace Testcontainers.Tests;

public sealed class SslNotSupportedTest
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void WithSslThrowsInvalidOperationException()
    {
        var certificateFilePath = Certificates.Instance.GetFilePath("server", "server.crt");
        var certificateKeyFilePath = Certificates.Instance.GetFilePath("server", "server.key");
        var exception = Assert.Throws<InvalidOperationException>(() => new ContainerBuilder().WithSsl(certificateFilePath, certificateKeyFilePath).Build());
        Assert.Equal("The module does not support SSL.", exception.Message);
    }
}