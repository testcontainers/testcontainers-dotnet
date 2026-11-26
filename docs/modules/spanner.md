# Google Cloud Spanner

[Google Cloud Spanner](https://cloud.google.com/spanner) is a fully managed, horizontally scalable relational database. The module runs the [Cloud Spanner emulator](https://cloud.google.com/spanner/docs/emulator), which keeps its data in memory.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Spanner
```

You can start a Spanner container instance from any .NET application. This example uses the default configuration:

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.Spanner.Tests/SpannerContainerTest.cs:CreateSpannerContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Spanner.Tests/SpannerContainerTest.cs:UseSpannerContainer"
    ```

The emulator starts without an instance or a database. The test example creates an instance with the `emulator-config` instance configuration and a database with a table, then reads the table definition back. Use `GetEmulatorGrpcEndpoint()` as the endpoint of the Spanner client and `ChannelCredentials.Insecure` as the channel credentials. `GetEmulatorHttpEndpoint()` returns the base address of the REST API.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Spanner.Tests/Testcontainers.Spanner.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Default configuration

--8<-- "src/Testcontainers.Spanner/SpannerBuilder.Generated.md"
