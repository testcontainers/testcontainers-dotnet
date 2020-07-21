namespace DotNet.Testcontainers.Tests.Fixtures.Containers.Modules
{
  using DotNet.Testcontainers.Containers.Modules.Abstractions;

  public class ModuleFixture<T> where T : HostedServiceContainer
  {
    public ModuleFixture(T container)
    {
      this.Container = container;
    }

    public T Container { get; }
  }
}
