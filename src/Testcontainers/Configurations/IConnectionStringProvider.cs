namespace DotNet.Testcontainers.Configurations
{
  public interface IConnectionStringProvider
  {
    string GetConnectionString(ConnectionMode connectionMode = ConnectionMode.Host);
  }

  public enum ConnectionMode
  {
    Host,

    Container,
  }
}
