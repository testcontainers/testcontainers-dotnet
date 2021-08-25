namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public abstract class ModuleFixture<T> : IAsyncLifetime
    where T : TestcontainersContainer
  {
    protected ModuleFixture(T container)
    {
      this.Container = container;
    }

    public T Container { get; }

    public virtual Task InitializeAsync()
    {
      return Task.CompletedTask;
    }

    public virtual Task DisposeAsync()
    {
      return Task.CompletedTask;
    }
  }
}
