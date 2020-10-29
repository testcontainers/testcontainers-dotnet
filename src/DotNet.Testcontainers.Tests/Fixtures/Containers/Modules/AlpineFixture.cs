namespace DotNet.Testcontainers.Tests.Fixtures.Containers.Modules
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Modules;
  using Xunit;

  public class AlpineFixture : ModuleFixture<TestcontainersContainer>, IAsyncLifetime
  {
    public AlpineFixture()
      : base(new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("alpine")
        .Build())
    {
    }

    public Task InitializeAsync()
    {
      return Task.WhenAll(this.Container.StartAsync(), this.Container.StopAsync());
    }

    public Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
