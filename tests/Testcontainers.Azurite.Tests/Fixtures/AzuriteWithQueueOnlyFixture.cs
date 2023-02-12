namespace Testcontainers.Azurite.Tests.Fixtures;

[UsedImplicitly]
public sealed class AzuriteWithQueueOnlyFixture : AzuriteDefaultFixture
{
  public AzuriteWithQueueOnlyFixture()
    : base(builder => builder.WithServices(AzuriteServices.Queue))
  {
  }
}