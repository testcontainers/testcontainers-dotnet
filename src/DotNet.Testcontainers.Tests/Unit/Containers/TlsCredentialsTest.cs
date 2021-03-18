namespace DotNet.Testcontainers.Tests.Unit.Containers
{
  using System.IO;
  using Testcontainers.Containers.Configurations;
  using Xunit;

  public class TlsCredentialsTest
  {
    [Fact]
    public void InitializeCertificateCredentialsWithoutException()
    {
      var dockerCertDir = new DockerCertificatesDirectory(Path.Combine("Assets", "tls"));
      var certificateCredentials = new TlsCredentials(dockerCertDir.CaCertificate, dockerCertDir.ClientCertificate, false, false);
      Assert.NotNull(certificateCredentials);
    }
  }
}
