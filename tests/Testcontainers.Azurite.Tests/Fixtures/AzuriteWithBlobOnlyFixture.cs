namespace Testcontainers.Azurite.Tests.Fixtures;

[UsedImplicitly]
public sealed class AzuriteWithBlobOnlyFixture : AzuriteDefaultFixture
{
  public AzuriteWithBlobOnlyFixture()
    : base(builder => builder.WithServices(AzuriteServices.Blob))
  {
  }
}