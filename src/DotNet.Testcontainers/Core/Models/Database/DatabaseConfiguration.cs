namespace DotNet.Testcontainers.Core.Models.Database
{
  using System.Collections.Generic;

  public abstract class DatabaseConfiguration
  {
    protected DatabaseConfiguration(string image, int defaultPort)
    {
      this.DefaultPort = defaultPort;

      this.Port = defaultPort;

      this.Image = image;
    }

    public int DefaultPort { get; }

    public int Port { get; set; }

    public string Image { get; }

    public string Hostname { get; set; } = "localhost";

    public abstract string Database { get; set; }

    public abstract string Username { get; set; }

    public abstract string Password { get; set; }

    public abstract IReadOnlyDictionary<string, string> Environments { get; }
  }
}
