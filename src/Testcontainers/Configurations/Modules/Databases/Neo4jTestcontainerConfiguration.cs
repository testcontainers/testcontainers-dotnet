namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class Neo4jTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private string password = DefaultPassword;

    private const string Neo4jImage = "neo4j:latest";

    private const int BoltPort = 7687;

    private const string DefaultDatabase = "neo4j";

    private const string DefaultUsername = "neo4j";

    private const string DefaultPassword = "connect";

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jTestcontainerConfiguration" /> class.
    /// </summary>
    public Neo4jTestcontainerConfiguration()
      : this(Neo4jImage)
    {
      this.Environments["NEO4JLABS_PLUGINS"] = "[\"apoc\"]";
      this.Environments["NEO4J_ACCEPT_LICENSE_AGREEMENT"] = "yes";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public Neo4jTestcontainerConfiguration(string image)
      : base(image, BoltPort)
    {
    }

    /// <inheritdoc />
    public override string Database => DefaultDatabase;

    /// <inheritdoc />
    public override string Username => DefaultUsername;

    /// <inheritdoc />
    public override string Password
    {
      get => this.password;

      set
      {
        this.password = value ?? DefaultPassword;

        this.Environments["NEO4J_AUTH"] = $"{this.Username}/{this.password}";
      }
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilPortIsAvailable(this.DefaultPort);
  }
}
