namespace DotNet.Testcontainers.Tests.Fixtures.Containers.Modules
{
  using DotNet.Testcontainers.Containers.Modules;

  public abstract class ModuleFixture<T> where T : TestcontainersContainer
  {
    public ModuleFixture(T container)
    {
      this.Container = container;
    }

    public T Container { get; }
  }
}
