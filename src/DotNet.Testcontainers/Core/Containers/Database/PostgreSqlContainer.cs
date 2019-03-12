namespace DotNet.Testcontainers.Core.Containers.Database
{
  using DotNet.Testcontainers.Core.Models;

  public class PostgreSqlContainer : DatabaseContainer
  {
    internal PostgreSqlContainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"Server=127.0.0.1;Port=5432;Database={this.Database};User Id={this.Username};Password={this.Password};";
  }
}
