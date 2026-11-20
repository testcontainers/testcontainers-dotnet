# Google Cloud Spanner

[Google Cloud Spanner](https://cloud.google.com/spanner) is a fully managed, horizontally scalable relational database. The module runs the [Cloud Spanner emulator](https://cloud.google.com/spanner/docs/emulator), which keeps its data in memory.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Spanner
```

You can start a Spanner container instance from any .NET application. Here, we create a container instance for the test class.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.Spanner.Tests/SpannerContainerTest.cs:CreateSpannerContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Spanner.Tests/SpannerContainerTest.cs:UseSpannerContainer"
    ```

The emulator starts without any instance or database. The test example creates an instance with the emulator's `emulator-config` configuration and a database with a table, then reads the table definition back. To read and write data, use the clients of `Google.Cloud.Spanner.Data` or `Google.Cloud.Spanner.V1` the same way. Use `GetEmulatorGrpcEndpoint()` to configure the Spanner clients. It returns the endpoint in the `host:port` format, which can also be used as the value of the `SPANNER_EMULATOR_HOST` environment variable. `GetEmulatorHttpEndpoint()` returns the endpoint of the REST API.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Spanner.Tests/Testcontainers.Spanner.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Default configuration

--8<-- "src/Testcontainers.Spanner/SpannerBuilder.Generated.md"
