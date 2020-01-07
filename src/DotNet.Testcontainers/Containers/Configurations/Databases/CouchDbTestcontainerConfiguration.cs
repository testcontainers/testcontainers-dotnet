namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public sealed class CouchDbTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public CouchDbTestcontainerConfiguration() : base("couchdb:2.3.1", 5984)
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

    public override IWaitUntil WaitStrategy => new WaitUntilShellCommandsAreCompleted($"curl -s 'http://{this.Hostname}:{this.DefaultPort}'");
  }
}
