namespace DotNet.Testcontainers.Core.Models.Database
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;

  public abstract class DatabaseConfiguration
  {
    private readonly IDictionary<string, string> environments = new Dictionary<string, string>();

    protected DatabaseConfiguration(string image, int defaultPort)
    {
      this.Image = image;
      this.HostPort = defaultPort;
      this.ContainerPort = defaultPort;
    }

    public string Image { get; }

    public int HostPort { get; set; }

    public int ContainerPort { get; set; }

    public string Hostname { get; set; } = "localhost";

    public virtual string Database { get; set; }

    public virtual string Username { get; set; }

    public virtual string Password { get; set; }

    public virtual IReadOnlyDictionary<string, string> Environments => new ReadOnlyDictionary<string, string>(this.environments);

    protected void WithEnvironment(string setting, string value, Action<string> save)
    {
      save(value);
      this.environments.Add(setting, value);
    }
  }
}
