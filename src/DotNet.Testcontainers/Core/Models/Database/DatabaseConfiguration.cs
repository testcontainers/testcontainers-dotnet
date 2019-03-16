namespace DotNet.Testcontainers.Core.Models.Database
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;

  public abstract class DatabaseConfiguration
  {
    private readonly IDictionary<string, string> environments = new Dictionary<string, string>();

    protected DatabaseConfiguration(string image)
    {
      this.Image = image;
    }

    public int Port { get; set; }

    public string Image { get; }

    public string Hostname { get; set; } = "127.0.0.1";

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
