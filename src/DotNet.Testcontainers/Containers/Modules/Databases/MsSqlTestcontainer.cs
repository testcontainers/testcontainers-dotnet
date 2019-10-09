namespace DotNet.Testcontainers.Containers.Modules.Databases
{
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.Modules.Abstractions;

  public sealed class MsSqlTestcontainer : TestcontainerDatabase
  {
    internal MsSqlTestcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"Server={this.Hostname},{this.Port};Database={this.Database};User Id={this.Username};Password={this.Password};";
  }
}
