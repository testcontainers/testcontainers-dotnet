namespace DotNet.Testcontainers.Core.Containers
{
  using DotNet.Testcontainers.Core.Containers.Database;
  using DotNet.Testcontainers.Core.Models;

  public abstract class TestcontainerMessageBroker : Testcontainer
  {
    protected TestcontainerMessageBroker(TestcontainersConfiguration configuration) : base(configuration)
    {
    }
  }
}
