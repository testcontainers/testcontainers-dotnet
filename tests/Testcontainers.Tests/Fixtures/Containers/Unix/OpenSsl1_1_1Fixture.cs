namespace DotNet.Testcontainers.Tests.Fixtures
{
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class OpenSsl1_1_1Fixture : DockerMTls
  {
    public OpenSsl1_1_1Fixture() : base("20.10.18")
    {
    }
  }
}
