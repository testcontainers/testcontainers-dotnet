# Azure Event Hubs

Azure [Event Hubs](https://learn.microsoft.com/en-us/azure/event-hubs/overview-emulator) emulator⁠ is designed to offer a local development experience for Azure Event Hubs⁠, enabling you to develop and test code against the service in isolation, free from cloud interference.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.EventHubs
```

You can start an Azure Event Hubs container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.EventHubs.Tests/EventHubsContainerTest.cs:CreateEventHubsContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.EventHubs.Tests/EventHubsContainerTest.cs:UseEventHubsContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.EventHubs.Tests/Testcontainers.EventHubs.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Use a custom Azurite instance

The Event Hubs module depends on an Azurite container instance. The module automatically creates and configures the necessary resources and connects them. If you prefer to use your own instance, you can use the following method to configure the builder accordingly:

=== "Reuse Existing Resources"
    ```csharp
    --8<-- "tests/Testcontainers.EventHubs.Tests/EventHubsContainerTest.cs:ReuseExistingAzuriteContainer"
    ```