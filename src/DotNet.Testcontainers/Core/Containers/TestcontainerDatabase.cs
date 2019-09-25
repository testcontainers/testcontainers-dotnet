namespace DotNet.Testcontainers.Core.Containers
{
  using DotNet.Testcontainers.Core.Models;

  public abstract class TestcontainerDatabase : Testcontainer
  {
    protected TestcontainerDatabase(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public string Database { get; set; }
  }
}
