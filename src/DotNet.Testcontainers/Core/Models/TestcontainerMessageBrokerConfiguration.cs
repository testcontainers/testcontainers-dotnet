namespace DotNet.Testcontainers.Core.Models
{
  public abstract class TestcontainerMessageBrokerConfiguration : TestcontainerConfiguration
  {
    protected TestcontainerMessageBrokerConfiguration(string image, int defaultPort) : base(image, defaultPort)
    {
    }
  }
}
