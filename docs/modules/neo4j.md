# Neo4j

[Neo4j](https://neo4j.com/product/neo4j-graph-database/) is a graph database designed to work with nodes and edges. It is a ACID-compliant transactional graph database engine, and developers can communicate with it using the HTTP endpoint or by using the **Bolt** protocol.

The following example uses the following NuGet packages:

```console title="Install the NuGet dependencies"
dotnet add package Testcontainers.Neo4j
dotnet add package Neo4j.Driver
dotnet add package xunit
```

IDEs and editors may also require the following packages to run tests: `xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk`.

Copy and paste the following code into a new `.cs` test file within an existing test project.

```csharp
using Neo4j.Driver;
using Testcontainers.Neo4j;
using Xunit;

namespace TestcontainersModules;

public sealed class Neo4jContainerTest : IAsyncLifetime
{
    private readonly Neo4jContainer _neo4jContainer
        = new Neo4jBuilder().Build();

    [Fact]
    public async Task CanReadNeo4jDatabase()
    {
        const string database = "neo4j";

        await using var client = GraphDatabase.Driver(_neo4jContainer.GetConnectionString());

        await using var session = client.AsyncSession(cfg => cfg.WithDatabase(database));

        Assert.Equal(database, session.SessionConfig.Database);
    }

    public Task InitializeAsync()
        => _neo4jContainer.StartAsync();

    public Task DisposeAsync()
        => _neo4jContainer.DisposeAsync().AsTask();
}
```

To execute the tests, use the command `dotnet test` from a terminal.

[xunit]: https://xunit.net/
