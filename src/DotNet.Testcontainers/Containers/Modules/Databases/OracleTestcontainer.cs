namespace DotNet.Testcontainers.Containers.Modules.Databases
{
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.Modules.Abstractions;

  public sealed class OracleTestcontainer : TestcontainerDatabase
  {
    internal OracleTestcontainer(ITestcontainersConfiguration configuration) : base(configuration)
    {
    }

    public override string ConnectionString => $"Data Source={this.Hostname}:{this.Port};User id={this.Username};Password={this.Password};";
  }
}
