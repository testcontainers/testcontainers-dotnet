namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class CouchDbTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string CouchDbImage = "couchdb:2.3.1";

    private const int CouchDbPort = 5984;

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchDbTestcontainerConfiguration" /> class.
    /// </summary>
    public CouchDbTestcontainerConfiguration()
      : this(CouchDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchDbTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public CouchDbTestcontainerConfiguration(string image)
      : base(image, CouchDbPort)
    {
    }

    /// <inheritdoc />
    public override string Username
    {
      get => this.Environments["COUCHDB_USER"];
      set => this.Environments["COUCHDB_USER"] = value;
    }

    /// <inheritdoc />
    public override string Password
    {
      get => this.Environments["COUCHDB_PASSWORD"];
      set => this.Environments["COUCHDB_PASSWORD"] = value;
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted($"curl -s 'http://localhost:{this.DefaultPort}'");
  }
}
