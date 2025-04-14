# ClickHouse

[ClickHouse](https://clickhouse.com/) is a high-performance, column-oriented SQL database management system (DBMS) for online analytical processing (OLAP).

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.ClickHouse
```

You can start a ClickHouse container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Test class"
    ```csharp
    public sealed class ClickHouseContainerTest : IAsyncLifetime
    {
        private readonly ClickHouseContainer _clickHouseContainer = new ClickHouseBuilder().Build();

        public Task InitializeAsync()
        {
            return _clickHouseContainer.StartAsync();
        }

        public Task DisposeAsync()
        {
            return _clickHouseContainer.DisposeAsync().AsTask();
        }
    }
    ```

=== "Connecting to ClickHouse"
    ```csharp
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new ClickHouseConnection(_clickHouseContainer.GetConnectionString());

        // When
        connection.Open();

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
    }
    ```

Execute SQL script example:
=== "SQL Script"
    ```csharp
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "SELECT 1;";

        // When
        var execResult = await _clickHouseContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.ClickHouse.Tests/Testcontainers.ClickHouse.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"