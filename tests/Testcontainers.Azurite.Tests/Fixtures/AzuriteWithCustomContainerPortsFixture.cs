namespace Testcontainers.Azurite.Tests.Fixtures;

[UsedImplicitly]
public sealed class AzuriteWithCustomContainerPortsFixture : AzuriteDefaultFixture
{
  public AzuriteWithCustomContainerPortsFixture()
    : base(builder => builder
      .WithBlobPort(65501)
      .WithQueuePort(65502)
      .WithTablePort(65503))
  {
  }
}