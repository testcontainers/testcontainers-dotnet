namespace DotNet.Testcontainers.Core.Models.Database
{
  public sealed class PostgreSqlTestcontainerConfiguration : DatabaseConfiguration
  {
    public PostgreSqlTestcontainerConfiguration() : base("postgres:11.2")
    {
      this.Port = 5432;
    }

    public override string Database
    {
      get
      {
        return base.Database;
      }

      set
      {
        this.WithEnvironment("POSTGRES_DB", value, database => base.Database = database);
      }
    }

    public override string Username
    {
      get
      {
        return base.Username;
      }

      set
      {
        this.WithEnvironment("POSTGRES_USER", value, username => base.Username = username);
      }
    }

    public override string Password
    {
      get
      {
        return base.Password;
      }

      set
      {
        this.WithEnvironment("POSTGRES_PASSWORD", value, password => base.Password = password);
      }
    }
  }
}
