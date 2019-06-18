namespace DotNet.Testcontainers.Core.Models
{
  public abstract class TestcontainerDatabaseConfiguration : TestcontainerConfiguration
  {
    protected TestcontainerDatabaseConfiguration(string image, int defaultPort) : base(image, defaultPort)
    {
    }

    public virtual string Database { get; set; }
  }
}
