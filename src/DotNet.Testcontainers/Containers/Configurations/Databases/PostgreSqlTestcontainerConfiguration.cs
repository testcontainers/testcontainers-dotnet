namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public class PostgreSqlTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string PostgreSqlImage = "postgres:11.5";

    private const int PostgreSqlPort = 5432;

    public PostgreSqlTestcontainerConfiguration()
      : this(PostgreSqlImage)
    {
    }

    public PostgreSqlTestcontainerConfiguration(string image)
      : base(image, PostgreSqlPort)
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

    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted($"pg_isready -h 'localhost' -p '{this.DefaultPort}'");
  }
}
