namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public class CouchDbTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string CouchDbImage = "couchdb:2.3.1";

    private const int CouchDbPort = 5984;

    public CouchDbTestcontainerConfiguration()
      : this(CouchDbImage)
    {
    }

    public CouchDbTestcontainerConfiguration(string image)
      : base(image, CouchDbPort)
    {
    }

    public override string Username
    {
      get => this.Environments["COUCHDB_USER"];
      set => this.Environments["COUCHDB_USER"] = value;
    }

    public override string Password
    {
      get => this.Environments["COUCHDB_PASSWORD"];
      set => this.Environments["COUCHDB_PASSWORD"] = value;
    }

    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted($"curl -s 'http://localhost:{this.DefaultPort}'");
  }
}
