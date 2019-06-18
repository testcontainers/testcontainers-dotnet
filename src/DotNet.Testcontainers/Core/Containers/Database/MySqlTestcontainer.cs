namespace DotNet.Testcontainers.Core.Containers.Database
{
  using DotNet.Testcontainers.Core.Models;

  public sealed class MySqlTestcontainer : TestcontainerDatabase
  {
    internal MySqlTestcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"Server={this.Hostname};Port={this.Port};Database={this.Database};Uid={this.Username};Pwd={this.Password};";
  }
}
