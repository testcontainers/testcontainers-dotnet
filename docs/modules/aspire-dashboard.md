# Aspire Dashboard

The [Aspire dashboard](https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard/overview) is a standalone tool for viewing the logs, traces, and metrics that your apps emit over the OpenTelemetry Protocol (OTLP). It provides a web interface to explore telemetry during local development.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.AspireDashboard
```

You can start an Aspire Dashboard container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.AspireDashboard.Tests/AspireDashboardContainerTest.cs:UseAspireDashboardContainer"
    ```

The test example exports a trace to the dashboard over OTLP and queries the dashboard's telemetry API to confirm that the span was ingested. Use `GetOtlpGrpcAddress()` or `GetOtlpHttpAddress()` to configure the OpenTelemetry exporter in your .NET service, and `GetDashboardAddress()` to open the dashboard's web interface.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.AspireDashboard.Tests/Testcontainers.AspireDashboard.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"