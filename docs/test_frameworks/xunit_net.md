# Testing with xUnit.net

The [Testcontainers.Xunit](https://www.nuget.org/packages/Testcontainers.Xunit) package simplifies writing tests with containers in [xUnit.net](https://xunit.net). By leveraging xUnit.net's [shared context](https://xunit.net/docs/shared-context), this package automates the setup and teardown of test resources, creating and disposing of containers as needed. This reduces repetitive code and avoids common patterns that developers would otherwise need to implement repeatedly.

To get started, add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Xunit
```

## Creating an isolated test context

To create a new test resource instance for each test, inherit from the `ContainerTest<TBuilderEntity, TContainerEntity>` class. Each test resource instance is isolated and not shared across other tests, making this approach ideal for destructive operations that could interfere with other tests. You can access the generic `TContainerEntity` container instance through the `Container` property.

The example below demonstrates how to override the `Configure(TBuilderEntity)` method and pin the image version. This method allows you to configure the container instance specifically for your test case, with all container builder methods available. If your tests rely on a Testcontainers' module, the module's default configurations will be applied.

=== "Configure a Redis Container"
    ```csharp
    --8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest`1.cs:ConfigureRedisContainer"
    ```

!!!tip
    Always pin the image version to avoid flakiness. This ensures consistency and prevents unexpected behavior, as the `latest` tag may pointing to a new version.

The base class also receives an instance of xUnit.net's [ITestOutputHelper](https://xunit.net/docs/capturing-output) to capture and forward log messages to the running test.

Considering that xUnit.net runs tests in a deterministic natural sort order (like `Test1`, `Test2`, etc.), retrieving the Redis (string) value in the second test will always return `null` since a new test resource instance (Redis container) is created for each test.

=== "Run Tests"
    ```csharp
    --8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest`1.cs:RunTests"
    ```

If you check the output of `docker ps`, you will notice that three container instances in total are run, with two of them being Redis instances.

```title="List running containers"
PS C:\Sources\dotnet\testcontainers-dotnet> docker ps
CONTAINER ID   IMAGE                       COMMAND                  CREATED
be115f3df138   redis:7.0                   "docker-entrypoint.s…"   3 seconds ago
59349127f8c0   redis:7.0                   "docker-entrypoint.s…"   4 seconds ago
45fa02b3e997   testcontainers/ryuk:0.9.0   "/bin/ryuk"              4 seconds ago
```

## Creating a shared test context

Sometimes, creating and disposing of a test resource can be an expensive operation that you do not want to repeat for every test. By inheriting from the `ContainerFixture<TBuilderEntity, TContainerEntity>` class, you can share the test resource instance across all tests within the same test class.

xUnit.net's fixture implementation does not rely on the `ITestOutputHelper` interface to capture and forward log messages; instead, it expects an implementation of `IMessageSink`. Make sure your fixture's default constructor accepts the interface implementation and forwards it to the base class.

=== "Configure Redis Container"
    ```csharp
    --8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest`2.cs:ConfigureRedisContainer"
    ```

This ensures that the fixture is created only once for the entire test class, which also improves overall test performance. You must implement the `IClassFixture<TFixture>` interface with the previously created container fixture type in your test class and add the type as argument to the default constructor.

=== "Inject Redis Container"
    ```csharp
    --8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest`2.cs:InjectContainerFixture"
    ```

In this case, retrieving the Redis (string) value in the second test will no longer return `null`. Instead, it will return the value added in the first test.

=== "Run Tests"
    ```csharp
    --8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest`2.cs:RunTests"
    ```

The output of `docker ps` shows that, instead of two Redis containers, only one runs.

```title="List running containers"
PS C:\Sources\dotnet\testcontainers-dotnet> docker ps
CONTAINER ID   IMAGE                       COMMAND                  CREATED
d29a393816ce   redis:7.0                   "docker-entrypoint.s…"   3 seconds ago
e878f0b8f4bc   testcontainers/ryuk:0.9.0   "/bin/ryuk"              3 seconds ago
```

## Testing ADO.NET services

In addition to the two mentioned base classes, the package contains two more classes: `DbContainerTest` and `DbContainerFixture`, which behave identically but offer additional convenient features when working with services accessible through an ADO.NET provider.

Inherit from either the `DbContainerTest` or `DbContainerFixture` class and override the `Configure(TBuilderEntity)` method to configure your database service.

In this example, we use the default configuration of the PostgreSQL module. The container image capabilities are used to instantiate the database, schema, and test data. During startup, the PostgreSQL container runs SQL scripts placed under the `/docker-entrypoint-initdb.d/` directory automatically.

=== "Configure PostgreSQL Container"
```csharp
--8<-- "tests/Testcontainers.Xunit.Tests/PostgreSqlContainer.cs:ConfigurePostgreSqlContainer"
```

Inheriting from the database container test or fixture class requires you to implement the abstract `DbProviderFactory` property and resolve a compatible `DbProviderFactory` according to your ADO.NET service.

=== "Configure DbProviderFactory"
```csharp
--8<-- "tests/Testcontainers.Xunit.Tests/PostgreSqlContainer.cs:ConfigureDbProviderFactory"
```

!!! note

    Depending on how you initialize and access the database, it may be necessary to override the `ConnectionString` property and replace the default database name with the one actual in use.

After configuring the dependent ADO.NET service, you can add the necessary tests. In this case, we run an SQL `SELECT` statement to retrieve the first record from the `album` table.

=== "Run Tests"
```csharp
--8<-- "tests/Testcontainers.Xunit.Tests/PostgreSqlContainer.cs:RunTests"
```

--8<-- "docs/modules/_call_out_test_projects.txt"