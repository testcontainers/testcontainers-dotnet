namespace DotNet.Testcontainers.Core.Models
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Core.Wait;
  using DotNet.Testcontainers.Diagnostics;

  public abstract class TestcontainerConfiguration
  {
    protected TestcontainerConfiguration(string image, int defaultPort) : this(image, defaultPort, defaultPort)
    {
    }

    protected TestcontainerConfiguration(string image, int defaultPort, int port)
    {
      this.Image = image;
      this.DefaultPort = defaultPort;
      this.Port = port;
    }

    public string Image { get; }

    public int DefaultPort { get; }

    public int Port { get; set; }

    public virtual string Hostname { get; set; } = "localhost";

    public virtual string Username { get; set; }

    public virtual string Password { get; set; }

    public virtual IDictionary<string, string> Environments { get; set; } = new Dictionary<string, string>();

    public virtual IOutputConsumer OutputConsumer => null;

    public virtual IWaitUntil WaitStrategy => Wait.UntilContainerIsRunning();
  }
}
