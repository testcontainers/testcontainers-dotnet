namespace DotNet.Testcontainers.Containers.Modules.Databases
{
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.Modules.Abstractions;

  public sealed class MySqlTestcontainer : TestcontainerDatabase
  {
    internal MySqlTestcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"Server={this.Hostname};Port={this.Port};Database={this.Database};Uid={this.Username};Pwd={this.Password};";
  }
}
