namespace DotNet.Testcontainers.Core.Models
{
  public class DatabaseConfiguration
  {
    public virtual int Port { get; set; } = -1;

    public virtual string Hostname { get; set; } = "localhost";

    public virtual string Database { get; set; }

    public virtual string Username { get; set; }

    public virtual string Password { get; set; }
  }
}
