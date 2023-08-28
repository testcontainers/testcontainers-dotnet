namespace DotNet.Testcontainers.Tests.Fixtures
{
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class OpenSsl3_1Fixture : DockerMTls
  {
    public OpenSsl3_1Fixture() : base("24.0.5")
    {
    }
  }
}
