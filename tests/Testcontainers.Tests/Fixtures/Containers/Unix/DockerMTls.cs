namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Images;

  public abstract class DockerMTls : ProtectDockerDaemonSocket
  {
    public DockerMTls(string dockerImageVersion)
      : base(new ContainerBuilder("docker:" + dockerImageVersion + "-dind"))
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
