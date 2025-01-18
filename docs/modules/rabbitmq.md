# RabbitMQ

[RabbitMQ](https://www.rabbitmq.com/) is a message broker that enables reliable communication between distributed applications by managing and routing messages between them.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.RabbitMq
```

You can start a RabbitMQ container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.RabbitMq.Tests/RabbitMqContainerTest.cs:UseRabbitMqContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.RabbitMq.Tests/Testcontainers.RabbitMq.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"