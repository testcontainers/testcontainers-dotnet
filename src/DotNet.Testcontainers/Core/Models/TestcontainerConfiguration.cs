namespace DotNet.Testcontainers.Core.Models
{
  using System.Collections.Generic;

  public abstract class TestcontainerConfiguration
  {
    protected TestcontainerConfiguration(string image, int defaultPort)
    {
      this.Image = image;

      this.DefaultPort = defaultPort;

      this.Port = defaultPort;
    }

    public string Image { get; }

    public int DefaultPort { get; }

    public int Port { get; set; }

    public virtual string Hostname { get; set; } = "localhost";

    public virtual string Username { get; set; }

    public virtual string Password { get; set; }

    public virtual IDictionary<string, string> Environments { get; set; } = new Dictionary<string, string>();
  }
}
