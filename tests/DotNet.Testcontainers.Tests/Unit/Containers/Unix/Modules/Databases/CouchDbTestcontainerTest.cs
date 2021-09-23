namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Net;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class CouchDbTestcontainerTest : IClassFixture<CouchDbFixture>
  {
    private readonly CouchDbFixture couchDbFixture;

    public CouchDbTestcontainerTest(CouchDbFixture couchDbFixture)
    {
      this.couchDbFixture = couchDbFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var database = this.couchDbFixture.Connection.Database;

      // When
      var response = await database.PutAsync()
        .ConfigureAwait(false);

      // Then
      Assert.True(response.IsSuccess);
    }

    [Fact]
    public async Task ExecScriptInRunningContainer()
    {
      // Given
      const string script = @"
        #!/bin/bash
        echo executing
        curl -v -X PUT http://couchdb:couchdb@127.0.0.1:5984/mydatabase/ 
        curl -v -X PUT http://couchdb:couchdb@127.0.0.1:5984/mydatabase/""001"" -d '{ "" name "" : "" MyName "" }' 
        echo executed
        ";

      // When
      var results = await this.couchDbFixture.Container.ExecScriptAsync(script);

      // Then
      Assert.Equal(0, results.ExitCode);
      using var client = new WebClient();
      var response = client.DownloadString($"{this.couchDbFixture.Container.ConnectionString}/mydatabase/001");
      Assert.Contains("MyName", response);
    }
  }
}
