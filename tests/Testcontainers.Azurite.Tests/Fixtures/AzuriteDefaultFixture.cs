namespace Testcontainers.Azurite.Tests.Fixtures;

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
    Configuration = builder.Configuration;
    Container = builder.Build();
  }

  public AzuriteConfiguration Configuration { get; }

  public AzuriteContainer Container { get; }

  public Task InitializeAsync()
  {
    return Container.StartAsync();
  }

  public Task DisposeAsync()
  {
    return Container.DisposeAsync().AsTask();
  }
}