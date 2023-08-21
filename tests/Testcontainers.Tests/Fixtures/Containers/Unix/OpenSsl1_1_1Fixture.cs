namespace DotNet.Testcontainers.Tests.Fixtures
{
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class OpenSsl1_1_1Fixture : DockerMTls
  {
    public const string DockerVersion = "20.10.18";
    public OpenSsl1_1_1Fixture() : base(DockerVersion)
    {
    }
  }
}
