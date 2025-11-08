# Toxiproxy

[Toxiproxy](https://github.com/Shopify/toxiproxy) is a proxy to simulate network and system conditions for chaos and resiliency testing. It can simulate latency, timeouts, bandwidth limits, and connection issues between services.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Toxiproxy
```

You can start a Toxiproxy container instance from any .NET application. This example demonstrates how to test network conditions by proxying traffic through Toxiproxy to a Redis container. The test creates both containers in the same network and configures Toxiproxy to redirect traffic from the test host to the Redis container.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.Toxiproxy.Tests/ToxiproxyContainerTest.cs:CreateToxiproxyContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the containers. Both the Redis and Toxiproxy containers are started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the containers are removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Toxiproxy.Tests/ToxiproxyContainerTest.cs:UseToxiproxyContainer"
    ```

## How it works

To test network conditions with Toxiproxy, you need to configure the communication between your test host, Toxiproxy, and the service you want to test:

1. Place both containers in the same network so the Toxiproxy container and the service container (Redis in this example) can communicate with each other.

2. Configure the Toxiproxy proxy to redirect traffic from the test host to the service container. The proxy's `Listen` address specifies where Toxiproxy listens for incoming connections, and the `Upstream` address specifies the target service.

    === "Proxy configuration"
        ```csharp
        --8<-- "tests/Testcontainers.Toxiproxy.Tests/ToxiproxyContainerTest.cs:ProxyConfiguration"
        ```

3. Connect through Toxiproxy using its hostname and one of its proxied ports. The Toxiproxy module initializes `32` ports starting from `8666` that can be used to configure proxies and redirect traffic. In the example, the Redis connection uses the Toxiproxy container's hostname and port instead of connecting directly to Redis.

    === "Connect through toxiproxy"
        ```csharp
        --8<-- "tests/Testcontainers.Toxiproxy.Tests/ToxiproxyContainerTest.cs:ConnectThroughToxiproxy"
        ```

4. Apply toxics to the proxy to simulate network conditions. For example, a latency toxic to the downstream traffic.

    === "Toxic configuration"
        ```csharp
        --8<-- "tests/Testcontainers.Toxiproxy.Tests/ToxiproxyContainerTest.cs:ToxicConfiguration"
        ```

!!! tip
    Toxiproxy allows you to configure the `Toxicity` property, which determines the probability (0.0 to 1.0) that a toxic will be applied to the connection. A value of 1.0 means the toxic is applied 100% of the time, while 0.5 would apply it to approximately 50% of requests.

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Toxiproxy.Tests/Testcontainers.Toxiproxy.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"