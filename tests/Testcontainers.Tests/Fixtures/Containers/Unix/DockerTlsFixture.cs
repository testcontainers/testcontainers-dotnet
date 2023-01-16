namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Collections.Generic;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class DockerTlsFixture : ProtectDockerDaemonSocket
  {
    public DockerTlsFixture()
      : base(new TestcontainersBuilder<TestcontainersContainer>()
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
