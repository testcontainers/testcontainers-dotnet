namespace DotNet.Testcontainers.Core.Models
{
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Wait;

  public abstract class TestcontainerMessageBrokerConfiguration : TestcontainerConfiguration
  {
    protected TestcontainerMessageBrokerConfiguration(string image, int defaultPort) : base(image, defaultPort, TestcontainersNetworkService.GetAvailablePort())
    {
    }

    public override IWaitUntil WaitStrategy => Wait.UntilPortsAreAvailable(this.DefaultPort);
  }
}
