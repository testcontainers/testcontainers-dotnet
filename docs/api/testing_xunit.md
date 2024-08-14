# Testing with xUnit

The [Testcontainers.Xunit](https://www.nuget.org/packages/Testcontainers.Xunit) package provides classes that simplify writing tests using Docker containers with [xUnit](https://xunit.net). Starting and disposing containers is handled automatically.

## Use a container per test method

If you need a fresh container per test method, inherit `ContainerTest<TBuilderEntity, TContainerEntity>` and use its `Container` property. Testcontainers steps will be logged through the injected `ITestOutputHelper`.

```csharp
using System.Threading.Tasks;
using StackExchange.Redis;
using Testcontainers.Redis;
using Testcontainers.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace MyProject.Tests;

public sealed class RedisContainerTest(ITestOutputHelper output)
    : ContainerTest<RedisBuilder, RedisContainer>(output)
{
    protected override RedisBuilder Configure(RedisBuilder builder)
    {
        // 👇 Pin the Redis image to version 7.4.0 (alpine)
        return builder.WithImage("redis:7.4.0-alpine");
    }

    [Fact]
    public async Task Test1()
    {
        // 👆 A new fresh container is started before the test is run

        // 👇 get the connection string from the container
        var connectionString = Container.GetConnectionString();
        using var redis = ConnectionMultiplexer.Connect(connectionString);
        await redis.GetDatabase().StringSetAsync("key", "value");

        // 👇 The container is disposed after the test has finished running
    }

    [Fact]
    public async Task Test2()
    {
        // 👆 Another fresh container is started before the second test is run

        var connectionString = Container.GetConnectionString();
        using var redis = ConnectionMultiplexer.Connect(connectionString);
        string value = await redis.GetDatabase().StringGetAsync("key");
        Assert.Null(value); // 👈 value is null since the container is not shared

        // 👇 The container is disposed after the test has finished running
    }
}
```

## Use a container per test class

Using a container per test method might be too heavy, especially if the container is slow to start. You might want to share a single container across all tests of a single class by using the `ContainerFixture<TBuilderEntity, TContainerEntity>` class that can be injected into your test classes.

The fixture provides a `Container` property of type `TContainerEntity`.

Testcontainers steps will be logged through `IMessageSink`.

```csharp
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Testcontainers.Azurite;
using Testcontainers.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace MyProject.Tests;

// Inherit from ContainerFixture<TBuilderEntity, TContainerEntity>
public class AzuriteFixture(IMessageSink messageSink)
    : ContainerFixture<AzuriteBuilder, AzuriteContainer>(messageSink);

// The same container is used in all tests from the same class, so
// make sure that tests are independent of each other.
public abstract class AzuriteContainerTest(AzuriteFixture fixture)
    : IClassFixture<AzuriteFixture>
{
    private string ConnectionString => fixture.Container.GetConnectionString();

    [Fact]
    public void Test1()
    {
        var blobClient = new BlobServiceClient(ConnectionString);
        // use the blob client ...
    }

    [Fact]
    public void Test2()
    {
        var queueClient = new QueueServiceClient(ConnectionString);
        // use the queue client ...
    }
}
```

Here's a more more complete example setting up a PostgreSQL database shared across all tests of the same class.

```csharp
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Testcontainers.PostgreSql;
using Testcontainers.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace MyProject.Tests;

// Inherit from DbContainerFixture to set up a PostgreSQL Docker container
public class PostgreSqlFixture(IMessageSink messageSink)
    : DbContainerFixture<PostgreSqlBuilder, PostgreSqlContainer>(messageSink)
{
    // Specifying the DbProviderFactory is required for DbContainerFixture
    public override DbProviderFactory DbProviderFactory => NpgsqlFactory.Instance;

    protected override PostgreSqlBuilder Configure(PostgreSqlBuilder builder)
    {
        const string script = "https://github.com/lerocha/chinook-database/" +
                              "releases/download/v1.4.5/" +
                              "Chinook_PostgreSql_AutoIncrementPKs.sql";
        return builder
            // Pin the PostgreSQL image to version 16.4 (alpine)
            .WithImage("postgres:16.4-alpine")
            // Initialize the database with the Chinook sample database
            .WithResourceMapping(script, "/docker-entrypoint-initdb.d/init.sql");
    }

    // Match the database name with the one from the initialization script
    public override string ConnectionString
    {
        get
        {
            var builder = new NpgsqlConnectionStringBuilder(base.ConnectionString)
            {
                Database = "chinook_auto_increment",
            };
            return builder.ConnectionString;
        }
    }
}

// Inject PostgreSqlFixture into the test and don't forget to use IClassFixture
public sealed class PostgreSqlContainerTest(PostgreSqlFixture fixture)
    : IClassFixture<PostgreSqlFixture>
{
    [Fact]
    public async Task Test1()
    {
        // CreateCommand is available on the fixture on .NET 8 onwards
        using DbCommand command = fixture.CreateCommand("SELECT 42");
        object result = await command.ExecuteScalarAsync();
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task Test2()
    {
        // OpenConnection(Async) is available on the fixture on .NET 8 onwards
        using DbConnection connection = await fixture.OpenConnectionAsync();
        const string query = "SELECT title FROM album ORDER BY album_id";
        string title = await connection.QueryFirstAsync<string>(query);
        Assert.Equal("For Those About To Rock We Salute You", title);
    }
}
```

