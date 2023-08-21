namespace DotNet.Testcontainers.Tests.Fixtures
{
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class OpenSsl3_1Fixture : DockerMTls
  {
    public const string DockerVersion = "24.0.5";
    public OpenSsl3_1Fixture() : base(DockerVersion)
    {
    }
  }
}
