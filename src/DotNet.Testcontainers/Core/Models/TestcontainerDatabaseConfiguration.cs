namespace DotNet.Testcontainers.Core.Models
{
  using DotNet.Testcontainers.Core.Wait;

  public abstract class TestcontainerDatabaseConfiguration : TestcontainerConfiguration
  {
    protected TestcontainerDatabaseConfiguration(string image, int defaultPort) : base(image, defaultPort)
    {
    }

    public virtual string Database { get; set; }

    public override IWaitUntil WaitStrategy => Wait.UntilPortsAreAvailable(this.DefaultPort);
  }
}
