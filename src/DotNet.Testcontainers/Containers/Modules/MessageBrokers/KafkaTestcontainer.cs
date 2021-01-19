namespace DotNet.Testcontainers.Containers.Modules.MessageBrokers
{
  using DotNet.Testcontainers.Containers.Modules.Abstractions;
  using DotNet.Testcontainers.Containers.Configurations;

  public sealed class KafkaTestcontainer : HostedServiceContainer
  {
    internal KafkaTestcontainer(ITestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public string BootstrapServers => $"{this.Hostname}:{this.Port}";
  }
}
