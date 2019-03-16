namespace DotNet.Testcontainers.Core.Models.Database
{
  public sealed class MySqlTestcontainerConfiguration : DatabaseConfiguration
  {
    public MySqlTestcontainerConfiguration() : base("mysql:8.0.15", 3306)
    {
      this.WithEnvironment("MYSQL_ALLOW_EMPTY_PASSWORD", "yes", value => { });
    }

    public override string Database
    {
      set
      {
        this.WithEnvironment("MYSQL_DATABASE", value, database => base.Database = database);
      }
    }

    public override string Username
    {
      set
      {
        this.WithEnvironment("MYSQL_USER", value, username => base.Username = username);
      }
    }

    public override string Password
    {
      set
      {
        this.WithEnvironment("MYSQL_PASSWORD", value, password => base.Password = password);
      }
    }
  }
}
