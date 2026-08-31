# QuestDB

[QuestDB](https://questdb.com/) is a high-performance, open-source time-series database designed for fast ingestion and low-latency SQL queries. It exposes its SQL interface over the PostgreSQL wire protocol and ingests time-series data over the InfluxDB Line Protocol (ILP).

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.QuestDb
```

You can start a QuestDB container instance from any .NET application. To create and start a container instance with the default configuration, use the module-specific builder as shown below:

=== "Start a QuestDB container"
    ```csharp
    var questDbContainer = new QuestDbBuilder("questdb/questdb:10.0.1").Build();
    await questDbContainer.StartAsync();
    ```

The following example utilizes the [xUnit.net](/test_frameworks/xunit_net/) module to reduce overhead by automatically managing the lifecycle of the dependent container instance. It creates and starts the container using the module-specific builder and injects it as a shared class fixture into the test class.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.QuestDb.Tests/QuestDbContainerTest.cs:UseQuestDbContainer"
    ```

The test example creates a table and queries it over the PostgreSQL wire protocol, and ingests a record over ILP. Use `GetConnectionString()` to configure a PostgreSQL client such as [Npgsql](https://www.npgsql.org/), and `GetIlpAddress()` to configure the [QuestDB .NET client](https://questdb.com/docs/clients/ingest-dotnet/). The REST API and the Web Console are available at `GetBaseAddress()`.

The default configuration uses the username `quest` and password `quest`. Use `WithUsername(string)` and `WithPassword(string)` to configure different credentials.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.QuestDb.Tests/Testcontainers.QuestDb.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"