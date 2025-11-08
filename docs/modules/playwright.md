# Playwright

[Playwright](https://playwright.dev/) is a framework for web testing and automation. It allows testing across all modern rendering engines including Chromium, WebKit, and Firefox with a single API. This module provides pre-configured browser containers for automated testing.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Playwright
```

You can start a Playwright container instance from any .NET application. To create and start a container instance with the default configuration, use the module-specific builder as shown below:

=== "Start a Playwright container"
    ```csharp
    var playwrightContainer = new PlaywrightBuilder().Build();
    await playwrightContainer.StartAsync();
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

This example demonstrates the Playwright container accessing a web site running inside another container (using the [`testcontainers/helloworld`](https://github.com/testcontainers/helloworld) image). Both containers are assigned to a shared network (see the [Network configuration](#network-configuration) section) to enable communication between them.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Playwright.Tests/PlaywrightContainerTest.cs:UsePlaywrightContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Playwright.Tests/Testcontainers.Playwright.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Network configuration

The Playwright container is configured with a network that can be shared with other containers. This is useful when testing applications that need to communicate with other services. Use the `GetNetwork()` method to access the container's network:

```csharp
var helloWorldContainer = new ContainerBuilder()
    .WithNetwork(playwrightContainer.GetNetwork())
    .Build();
```