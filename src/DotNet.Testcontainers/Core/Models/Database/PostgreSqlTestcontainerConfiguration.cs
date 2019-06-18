namespace DotNet.Testcontainers.Core.Models.Database
{
  public sealed class PostgreSqlTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public PostgreSqlTestcontainerConfiguration() : base("postgres:11.2", 5432)
    {
    }

    public override string Database
    {
      get => this.environments["POSTGRES_DB"];
      set => this.environments["POSTGRES_DB"] = value;
    }

    public override string Username
    {
      get => this.environments["POSTGRES_USER"];
      set => this.environments["POSTGRES_USER"] = value;
    }

    public override string Password
    {
      get => this.environments["POSTGRES_PASSWORD"];
      set => this.environments["POSTGRES_PASSWORD"] = value;
    }
  }
}
