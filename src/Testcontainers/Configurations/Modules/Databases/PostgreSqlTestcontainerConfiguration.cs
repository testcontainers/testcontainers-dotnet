namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class PostgreSqlTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string PostgreSqlImage = "postgres:11.14";

    private const int PostgreSqlPort = 5432;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlTestcontainerConfiguration" /> class.
    /// </summary>
    public PostgreSqlTestcontainerConfiguration()
      : this(PostgreSqlImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public PostgreSqlTestcontainerConfiguration(string image)
      : base(image, PostgreSqlPort)
    {
    }

    /// <inheritdoc />
    public override string Database
    {
      get => this.Environments["POSTGRES_DB"];
      set => this.Environments["POSTGRES_DB"] = value;
    }

    /// <inheritdoc />
    public override string Username
    {
      get => this.Environments["POSTGRES_USER"];
      set => this.Environments["POSTGRES_USER"] = value;
    }

    /// <inheritdoc />
    public override string Password
    {
      get => this.Environments["POSTGRES_PASSWORD"];
      set => this.Environments["POSTGRES_PASSWORD"] = value;
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted("pg_isready", "--host", "localhost", "--port", $"{this.DefaultPort}");
  }
}
