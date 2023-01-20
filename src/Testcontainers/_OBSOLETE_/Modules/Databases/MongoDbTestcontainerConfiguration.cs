namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class MongoDbTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string MongoDbImage = "mongo:5.0.6";

    private const int MongoDbPort = 27017;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbTestcontainerConfiguration" /> class.
    /// </summary>
    public MongoDbTestcontainerConfiguration()
      : this(MongoDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public MongoDbTestcontainerConfiguration(string image)
      : base(image, MongoDbPort)
    {
    }

    /// <inheritdoc />
    public override string Database
    {
      get => this.Environments["MONGO_INITDB_DATABASE"];
      set => this.Environments["MONGO_INITDB_DATABASE"] = value;
    }

    /// <inheritdoc />
    public override string Username
    {
      get => this.Environments["MONGO_INITDB_ROOT_USERNAME"];
      set => this.Environments["MONGO_INITDB_ROOT_USERNAME"] = value;
    }

    /// <inheritdoc />
    public override string Password
    {
      get => this.Environments["MONGO_INITDB_ROOT_PASSWORD"];
      set => this.Environments["MONGO_INITDB_ROOT_PASSWORD"] = value;
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted("mongo", $"localhost:{this.DefaultPort}", "--eval", "db.runCommand(\"ping\").ok", "--quiet");
  }
}
