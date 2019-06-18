namespace DotNet.Testcontainers.Core.Models
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Diagnostics.CodeAnalysis;

  public abstract class TestcontainerConfiguration
  {
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed.")]
    protected readonly IDictionary<string, string> environments = new Dictionary<string, string>();

    protected TestcontainerConfiguration(string image, int defaultPort)
    {
      this.Image = image;

      this.DefaultPort = defaultPort;

      this.Port = defaultPort;
    }

    public int DefaultPort { get; }

    public int Port { get; set; }

    public string Image { get; }

    public virtual string Hostname { get; set; } = "localhost";

    public virtual string Username { get; set; }

    public virtual string Password { get; set; }

    public IReadOnlyDictionary<string, string> Environments => new ReadOnlyDictionary<string, string>(this.environments);
  }
}
