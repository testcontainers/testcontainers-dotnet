namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Collections.Generic;
  using System.IO;
  using DotNet.Testcontainers.Builders;
  using Org.BouncyCastle.OpenSsl;

  public abstract class DockerMTls : ProtectDockerDaemonSocket
  {
    public DockerMTls(string dockerImageVersion)
      : base(new ContainerBuilder(), dockerImageVersion)
    {
    }

    public object ClientCertificateKey()
    {
      var path = Path.Combine(_hostCertsDirectoryPath, "client", "key.pem");
      using (var keyFileStream = new StreamReader(path))
      {
        return new PemReader(keyFileStream).ReadObject();
      }
    }

    public override IList<string> CustomProperties
    {
      get
      {
        var customProperties = base.CustomProperties;
        customProperties.Add("docker.tls=false");
        customProperties.Add("docker.tls.verify=true");
        return customProperties;
      }
    }
  }
}
