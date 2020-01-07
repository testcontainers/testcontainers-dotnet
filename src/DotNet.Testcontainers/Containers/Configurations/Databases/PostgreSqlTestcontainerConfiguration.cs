namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public sealed class PostgreSqlTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public PostgreSqlTestcontainerConfiguration() : base("postgres:11.5", 5432)
    {
    }

    public override string Database
    {
      get => this.Environments["POSTGRES_DB"];
      set => this.Environments["POSTGRES_DB"] = value;
    }

    public override string Username
    {
      get => this.Environments["POSTGRES_USER"];
      set => this.Environments["POSTGRES_USER"] = value;
    }

    public override string Password
    {
      get => this.Environments["POSTGRES_PASSWORD"];
      set => this.Environments["POSTGRES_PASSWORD"] = value;
    }

    public override IWaitUntil WaitStrategy => new WaitUntilShellCommandsAreCompleted($"pg_isready -h '{this.Hostname}' -p '{this.DefaultPort}'");
  }
}
