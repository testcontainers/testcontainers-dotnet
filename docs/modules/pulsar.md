# Apache Pulsar

[Apache Pulsar](https://pulsar.apache.org/) is a distributed messaging and event streaming platform designed for high-throughput, low-latency data processing across multiple topics.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Pulsar
```

You can start a Apache Pulsar container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.Pulsar.Tests/PulsarContainerTest.cs:CreatePulsarContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Pulsar.Tests/PulsarContainerTest.cs:UsePulsarContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Pulsar.Tests/Testcontainers.Pulsar.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Access Apache Pulsar

```csharp title="Gets the Pulsar broker URL"
string pulsarBrokerUrl = _pulsarContainer.GetPulsarBrokerUrl();
```

```csharp title="Gets the Pulsar service URL"
string pulsarServiceUrl = _pulsarContainer.GetHttpServiceUrl();
```

## Enable token authentication

If you need to use token authentication, use the following builder configuration to enable authentication:

```csharp
PulsarContainer _pulsarContainer = PulsarBuilder().WithTokenAuthentication().Build();
```

Start the container and obtain an authentication token with a specified expiration time

```csharp
var authToken = await container.CreateAuthenticationTokenAsync(TimeSpan.FromHours(1))
    .ConfigureAwait(false);
```

Alternatively, set the token to never expire

```csharp
var authToken = await container.CreateAuthenticationTokenAsync(Timeout.InfiniteTimeSpan)
    .ConfigureAwait(false);
```

## Enable Pulsar Functions

If you need to use Pulsar Functions, use the following builder configuration to enable it:

```csharp
PulsarContainer _pulsarContainer = PulsarBuilder().WithFunctions().Build();
```