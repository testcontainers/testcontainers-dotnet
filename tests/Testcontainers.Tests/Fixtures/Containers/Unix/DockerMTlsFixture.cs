namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class DockerMTlsFixture : ProtectDockerDaemonSocket
  {
    public DockerMTlsFixture()
      : base(new ContainerBuilder())
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
