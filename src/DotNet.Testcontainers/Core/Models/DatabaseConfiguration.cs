namespace DotNet.Testcontainers.Core.Models
{
  public class DatabaseConfiguration
  {
    public virtual string Hostname { get; set; } = "localhost";

    public virtual string Port { get; set; }

    public virtual string Database { get; set; }

    public virtual string Username { get; set; }

    public virtual string Password { get; set; }
  }
}
