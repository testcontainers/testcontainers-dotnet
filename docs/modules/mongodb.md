# MongoDB

[MongoDB](https://www.mongodb.com/what-is-mongodb) is a cross-platform document-oriented database. MongoDB's document model is simple for developers to use within their applications, while still providing all the complex capabilities of traditional relational databases.

The following example uses the following NuGet packages:

```console title="Install the NuGet dependencies"
dotnet add package Testcontainers.MongoDb
dotnet add package MongoDB.Driver
dotnet add package xunit
```

IDEs and editors may also require the following packages to run tests: `xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk`.

Copy and paste the following code into a new `.cs` test file within an existing test project.

```csharp
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;

namespace TestcontainersModules;

public sealed class MongoDbContainerTest : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer =
        new MongoDbBuilder().Build();

    [Fact]
    public async Task ReadFromMongoDbDatabase()
    {
        var client = new MongoClient(_mongoDbContainer.GetConnectionString());

        using var databases = await client.ListDatabasesAsync();

        Assert.True(await databases.AnyAsync());
    }

    public Task InitializeAsync()
        => _mongoDbContainer.StartAsync();

    public Task DisposeAsync()
        => _mongoDbContainer.DisposeAsync().AsTask();
}
```

To execute the tests, use the command `dotnet test` from a terminal.

[xunit]: https://xunit.net/
