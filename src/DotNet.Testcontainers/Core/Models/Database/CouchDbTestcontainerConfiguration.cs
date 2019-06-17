namespace DotNet.Testcontainers.Core.Models.Database
{
  public class CouchDbTestcontainerConfiguration : DatabaseConfiguration
  {
    public CouchDbTestcontainerConfiguration() : base("couchdb:2.3.1", 5984)
    {
    }

    public override string Database { get; set; }

    public override string Username
    {
      get => this.environments["COUCHDB_USER"];
      set => this.environments["COUCHDB_USER"] = value;
    }

    public override string Password
    {
      get => this.environments["COUCHDB_PASSWORD"];
      set => this.environments["COUCHDB_PASSWORD"] = value;
    }
  }
}
