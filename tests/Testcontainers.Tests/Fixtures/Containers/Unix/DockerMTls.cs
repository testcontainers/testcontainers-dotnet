namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;

  public abstract class DockerMTls : ProtectDockerDaemonSocket
  {
    public DockerMTls(string dockerImageVersion)
      : base(new ContainerBuilder(), dockerImageVersion)
    {
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
