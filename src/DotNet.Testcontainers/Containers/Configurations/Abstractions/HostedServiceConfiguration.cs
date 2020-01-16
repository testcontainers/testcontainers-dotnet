namespace DotNet.Testcontainers.Containers.Configurations.Abstractions
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  /// <summary>
  /// This class represents an extended Testcontainer configuration for modules. It is convenient for common configurations to provide a module with all necessary properties, without creating a new configuration again and again.
  /// </summary>
  public abstract class HostedServiceConfiguration
  {
    protected HostedServiceConfiguration(string image, int defaultPort) : this(image, defaultPort, defaultPort)
    {
    }

    protected HostedServiceConfiguration(string image, int defaultPort, int port)
    {
      this.Image = image;
      this.DefaultPort = defaultPort;
      this.Port = port;
    }

    public string Image { get; }

    public int DefaultPort { get; }

    public int Port { get; set; }

    public virtual string Username { get; set; }

    public virtual string Password { get; set; }

    public virtual IDictionary<string, string> Environments { get; } = new Dictionary<string, string>();

    public virtual IOutputConsumer OutputConsumer => OutputConsumerNull.Consumer;

    public virtual IWaitUntil WaitStrategy => Wait.UntilContainerIsRunning();
  }
}
