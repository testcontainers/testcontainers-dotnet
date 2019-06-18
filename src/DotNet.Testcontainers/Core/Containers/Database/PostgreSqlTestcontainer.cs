namespace DotNet.Testcontainers.Core.Containers.Database
{
  using DotNet.Testcontainers.Core.Models;

  public sealed class PostgreSqlTestcontainer : TestcontainerDatabase
  {
    internal PostgreSqlTestcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"Server={this.Hostname};Port={this.Port};Database={this.Database};User Id={this.Username};Password={this.Password};";
  }
}
