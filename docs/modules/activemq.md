# Apache ActiveMQ Artemis

[Apache ActiveMQ Artemis](https://activemq.apache.org/components/artemis/) is an open source project to build a multi-protocol, embeddable, very high performance, clustered, asynchronous messaging system.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.ActiveMq
```

You can start an Apache ActiveMQ Artemis container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Base test class"
    ```csharp
    --8<-- "tests/Testcontainers.ActiveMq.Tests/ArtemisContainerTest.cs:UseArtemisContainer"
    }
    ```
=== "Without auth"
    ```csharp
    --8<-- "tests/Testcontainers.ActiveMq.Tests/ArtemisContainerTest.cs:UseArtemisContainerNoAuth"
    ```
=== "Default credentials"
    ```csharp
    --8<-- "tests/Testcontainers.ActiveMq.Tests/ArtemisContainerTest.cs:UseArtemisContainerDefaultAuth"
    ```
=== "Custom credentials"
    ```csharp
    --8<-- "tests/Testcontainers.ActiveMq.Tests/ArtemisContainerTest.cs:UseArtemisContainerCustomAuth"
    ```

Connect to the container and produce a message:

=== "EstablishesConnection"
    ```csharp
    --8<-- "tests/Testcontainers.ActiveMq.Tests/ArtemisContainerTest.cs:ArtemisContainerEstablishesConnection"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.ActiveMq.Tests/Testcontainers.ActiveMq.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"