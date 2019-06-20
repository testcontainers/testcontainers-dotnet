namespace DotNet.Testcontainers.Core.Models.Database
{
  public sealed class MySqlTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public MySqlTestcontainerConfiguration() : base("mysql:8.0.15", 3306)
    {
      this.Environments["MYSQL_ALLOW_EMPTY_PASSWORD"] = "yes";
    }

    public override string Database
    {
      get => this.Environments["MYSQL_DATABASE"];
      set => this.Environments["MYSQL_DATABASE"] = value;
    }

    public override string Username
    {
      get => this.Environments["MYSQL_USER"];
      set => this.Environments["MYSQL_USER"] = value;
    }

    public override string Password
    {
      get => this.Environments["MYSQL_PASSWORD"];
      set => this.Environments["MYSQL_PASSWORD"] = value;
    }
  }
}
