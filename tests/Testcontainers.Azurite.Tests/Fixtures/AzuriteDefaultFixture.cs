namespace Testcontainers.Azurite.Tests.Fixtures
{
  [UsedImplicitly]
  public class AzuriteDefaultFixture : IAsyncLifetime
  {
    public AzuriteDefaultFixture()
      : this(builder => builder)
    {
    }

    protected AzuriteDefaultFixture(Func<AzuriteBuilder, AzuriteBuilder> modifier)
    {
      var builder = modifier(new AzuriteBuilder());
      this.Container = builder.Build();
    }

    public AzuriteContainer Container { get; }

    public Task InitializeAsync()
    {
      return this.Container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
