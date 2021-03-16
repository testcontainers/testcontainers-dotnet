namespace DotNet.Testcontainers.Tests.Unit.Containers
{
  using System.IO;
  using Testcontainers.Containers.Configurations;
  using Xunit;

  public class PemCertificateCredentialsTest
  {
    [Fact]
    public void InitializeCertificateCredentialsWithoutException()
    {
      var dockerCertDir = new DockerCertDir(Path.Combine("Assets", "tls"));
      var certificateCredentials = new PemCertificateCredentials(dockerCertDir, true);
      Assert.NotNull(certificateCredentials);
    }
  }
}
