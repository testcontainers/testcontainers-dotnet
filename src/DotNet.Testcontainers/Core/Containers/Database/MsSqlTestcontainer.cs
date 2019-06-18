namespace DotNet.Testcontainers.Core.Containers.Database
{
  using DotNet.Testcontainers.Core.Models;

  public sealed class MsSqlTestcontainer : TestcontainerDatabase
  {
    internal MsSqlTestcontainer(TestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"Server={this.Hostname},{this.Port};Database={this.Database};User Id={this.Username};Password={this.Password};";
  }
}
