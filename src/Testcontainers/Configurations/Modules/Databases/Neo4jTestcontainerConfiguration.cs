namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Linq;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class Neo4jTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string Neo4jImage = "neo4j:4.4.11";

    private const int BoltPort = 7687;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jTestcontainerConfiguration" /> class.
    /// </summary>
    public Neo4jTestcontainerConfiguration()
      : this(Neo4jImage) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public Neo4jTestcontainerConfiguration(string image)
      : base(image, BoltPort)
    {
      this.Environments["NEO4JLABS_PLUGINS"] = "[\"apoc\"]";
      this.Environments["NEO4J_ACCEPT_LICENSE_AGREEMENT"] = "yes";
      this.Password = "connect";
    }

    /// <inheritdoc />
    public override string Database
    {
      get => "neo4j";
      set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string Username
    {
      get => "neo4j";
      set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string Password
    {
      get => this.Environments["NEO4J_AUTH"].Split(new[] { '/' }, 2, StringSplitOptions.RemoveEmptyEntries).Last();
      set => this.Environments["NEO4J_AUTH"] = string.Join("/", this.Username, value);
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilPortIsAvailable(this.DefaultPort);
  }
}
