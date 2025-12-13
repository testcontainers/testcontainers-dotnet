namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class DockerTlsFixture : ProtectDockerDaemonSocket
  {
    public DockerTlsFixture()
      : base(new ContainerBuilder(new DockerImage("docker", null, "29.0.0-dind"))
        .WithCommand("--tlsverify=false"))
    {
    }

    public override IList<string> CustomProperties
    {
      get
      {
        var customProperties = base.CustomProperties;
        customProperties.Add("docker.tls=true");
        customProperties.Add("docker.tls.verify=false");
        return customProperties;
      }
    }
  }
}
