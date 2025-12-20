# Grafana

[Grafana](https://grafana.com/) is an open-source platform for monitoring, visualization, and analytics. It allows you to query, visualize, alert on, and explore metrics, logs, and traces from various data sources through customizable dashboards.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Grafana
```

You can start a Grafana container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.Grafana.Tests/GrafanaContainerTest.cs:CreateGrafanaContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Grafana.Tests/GrafanaContainerTest.cs:UseGrafanaContainer"
    ```

The test example queries the Grafana API endpoint `GET /api/org/` to retrieve the current organization. This API endpoint requires authentication, so the HTTP client's authorization header is set with Basic authentication using the username and password configured through the Grafana builder. The default configuration uses the username `admin` and password `admin`.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Grafana.Tests/Testcontainers.Grafana.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Enable anonymous access

Developers can enable anonymous access using the Grafana builder API `WithAnonymousAccessEnabled()`. This will enable anonymous access and no authentication is necessary to access Grafana:

```csharp
GrafanaContainer _grafanaContainer = new GrafanaBuilder("grafana/grafana:12.2").WithAnonymousAccessEnabled().Build();
```