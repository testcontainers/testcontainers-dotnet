# Azure EventHubs

[Azure EventHubs](https://learn.microsoft.com/en-us/azure/event-hubs/event-hubs-about) is a native data-streaming service in the cloud that can stream millions of events per second, with low latency, from any source to any destination. Event Hubs is compatible with Apache Kafka. It enables you to run existing Kafka workloads without any code changes.
In this module, you will learn how to use Testcontainers to start an [Azure EventHubs emulator](https://learn.microsoft.com/en-us/azure/event-hubs/overview-emulator) container for testing. To be able to use the Azure EventHubs emulator, you need to accept the [Microsoft Event Hubs Emulator License](https://github.com/Azure/azure-event-hubs-emulator-installer/blob/main/EMULATOR_EULA.md).

!!!Warning

    In the official documentation, there are **known limitations** to the Azure EventHubs emulator. You can find it [here](https://learn.microsoft.com/en-us/azure/event-hubs/overview-emulator#known-limitations).

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.EventHubs
```

You can start a Azure EventHubs emulator instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

To create a container instance with minimal configuration, use the following code:

=== "Create initial configuration JSON"
```csharp
--8<-- "tests/Testcontainers.EventHubs.Tests/EventHubsContainerTest.cs:MinimalConfigurationJson"
```

=== "Create Container Instance"
```csharp
--8<-- "tests/Testcontainers.EventHubs.Tests/EventHubsContainerTest.cs:MinimalConfigurationEventHubs"
```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
```csharp
--8<-- "tests/Testcontainers.EventHubs.Tests/EventHubsContainerTest.cs:EventHubsUsage"
```
