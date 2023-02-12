namespace Testcontainers.Azurite.Tests.Fixtures;

[UsedImplicitly]
public sealed class AzuriteWithTableOnlyFixture : AzuriteDefaultFixture
{
  public AzuriteWithTableOnlyFixture()
    : base(builder => builder.WithServices(AzuriteServices.Table))
  {
  }
}