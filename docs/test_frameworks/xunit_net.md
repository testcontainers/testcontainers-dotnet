# Testing with xUnit.net

The [Testcontainers.Xunit](https://www.nuget.org/packages/Testcontainers.Xunit) package simplifies writing tests with containers in [xUnit.net](https://xunit.net). By leveraging xUnit.net's [shared context](https://xunit.net/docs/shared-context), this package automates the setup and teardown of test resources, creating and disposing of containers as needed. This approach reduces repetitive code and avoids common patterns that developers would otherwise need to implement over and over again.

Integrating Testcontainers with xUnit.net tests reduces boilerplate code, leading to cleaner and more maintainable tests while efficiently managing resources.

## Creating isolated test context

To create a new test resource instance for each test, inherit from the `ContainerTest<TBuilderEntity, TContainerEntity>` class. Each test resource instance is isolated and not shared among other tests, making this approach ideal for destructive operations that could interfere with other tests. You can access the generic `TContainerEntity` container instance using the `Container` property.

The following example demonstrates how to override the `Configure(TBuilderEntity)` method and pin the image version. This method allows you to configure the container instance specifically for your use and test case, with all container builder methods available. If your tests rely on a Testcontainers module, the module's default configurations are applied.

=== "Configure Redis Container"
    ```csharp
    --8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest.cs:ConfigureRedisContainer"
    ```

!!!tip

    Always override and pin the image version, whether you are using the generic or module container builder.

The base class also receives an instance of xUnit.net's [ITestOutputHelper](https://xunit.net/docs/capturing-output) to capture and forward log messages to the actual running test.

Considering that xUnit.net runs tests in a deterministic natural sort order (e.g., `Test1`, `Test2`, etc.), getting the string value in the second test will always return `null` because a new test resource instance (Redis container) is created for each test.

=== "Run Tests"
    ```csharp
    --8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest.cs:RunTests"
    ```

If you look into the output of `docker ps`, you will notice the tests run three container instances in total, with two of them being Redis instances.

```title="List running containers"
PS C:\Sources\dotnet\testcontainers-dotnet> docker ps
CONTAINER ID   IMAGE                       COMMAND                  CREATED
be115f3df138   redis:7.0                   "docker-entrypoint.s…"   3 seconds ago
59349127f8c0   redis:7.0                   "docker-entrypoint.s…"   4 seconds ago
45fa02b3e997   testcontainers/ryuk:0.9.0   "/bin/ryuk"              4 seconds ago
```

## Creating a shared test context