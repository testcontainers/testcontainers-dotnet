# Testing with TUnit

The [Testcontainers.TUnit](https://www.nuget.org/packages/Testcontainers.TUnit) package simplifies writing tests with containers in [TUnit](https://tunit.dev). By leveraging TUnit's [test lifecycle](https://tunit.dev/docs/writing-tests/lifecycle) and [injectable class data sources](https://tunit.dev/docs/writing-tests/class-data-source), this package automates the setup and teardown of test resources, creating and disposing of containers as needed. This reduces repetitive code and avoids common patterns that developers would otherwise need to implement repeatedly.

To get started, add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.TUnit
```

## Creating an isolated test context

To create a new test resource instance for each test, inherit from the `ContainerTest<TBuilderEntity, TContainerEntity>` class. TUnit creates a new instance of the test class for every test, so each test resource instance is isolated and not shared across other tests, making this approach ideal for destructive operations that could interfere with other tests. You can access the generic `TContainerEntity` container instance through the `Container` property.

The example below demonstrates how to override the `Configure()` method and pin the image version. This method allows you to configure the container instance specifically for your test case, with all container builder methods available. If your tests rely on a Testcontainers' module, the module's default configurations will be applied.

=== "Configure a Redis Container"
    ```csharp
    --8<-- "tests/Testcontainers.TUnit.Tests/RedisContainerTest`1.cs:ConfigureRedisContainer"
    ```

!!! tip

    Always pin the image version to avoid flakiness. This ensures consistency and prevents unexpected behavior, as the `latest` tag may pointing to a new version.

The base class automatically forwards Testcontainers' log messages to the output of the running test. Container startup honors the test's cancellation token, so a canceled or timed out test does not leave a container start running in the background.

Considering that each test gets its own test resource instance (Redis container), retrieving the Redis (string) value in the second test will always return `null`, regardless of the order in which TUnit runs the tests.

=== "Run Tests"
    ```csharp
    --8<-- "tests/Testcontainers.TUnit.Tests/RedisContainerTest`1.cs:RunTests"
    ```

If you check the output of `docker ps`, you will notice that three container instances in total are run, with two of them being Redis instances.

```text title="List running containers"
PS C:\Sources\dotnet\testcontainers-dotnet> docker ps
CONTAINER ID   IMAGE                       COMMAND                  CREATED
be115f3df138   redis:7.0                   "docker-entrypoint.s…"   3 seconds ago
59349127f8c0   redis:7.0                   "docker-entrypoint.s…"   4 seconds ago
45fa02b3e997   testcontainers/ryuk:0.14.0   "/bin/ryuk"             4 seconds ago
```

## Creating a shared test context

Sometimes, creating and disposing of a test resource can be an expensive operation that you do not want to repeat for every test. By inheriting from the `ContainerFixture<TBuilderEntity, TContainerEntity>` class, you can share the test resource instance across all tests within the same test class, the same assembly, or even the entire test session.

=== "Configure Redis Container"
    ```csharp
    --8<-- "tests/Testcontainers.TUnit.Tests/RedisContainerTest`2.cs:ConfigureRedisContainer"
    ```

TUnit injects the fixture through the `ClassDataSource<TFixture>` attribute. The `Shared` argument controls the lifetime of the fixture: `SharedType.PerClass` creates the fixture once for the entire test class, `SharedType.PerAssembly` and `SharedType.PerTestSession` widen the scope accordingly, and `SharedType.Keyed` shares the fixture among all tests that use the same key. TUnit starts the container before the first test that uses the fixture runs and disposes of it after the last test in the chosen scope completes. Add the attribute to your test class and accept the fixture as a constructor parameter, or annotate a `required` property with it instead.

=== "Inject Redis Container"
    ```csharp
    --8<-- "tests/Testcontainers.TUnit.Tests/RedisContainerTest`2.cs:InjectContainerFixture"
    ```

TUnit runs tests in parallel by default. In this case, retrieving the Redis (string) value in the second test depends on the value the first test adds. The `DependsOn` attribute ensures the second test does not start before the first one has finished, without sacrificing parallelism for the remaining tests. The Redis (string) value will therefore no longer be `null`; instead, it will return the value added in the first test.

=== "Run Tests"
    ```csharp
    --8<-- "tests/Testcontainers.TUnit.Tests/RedisContainerTest`2.cs:RunTests"
    ```

The output of `docker ps` shows that, instead of two Redis containers, only one runs.

```text title="List running containers"
PS C:\Sources\dotnet\testcontainers-dotnet> docker ps
CONTAINER ID   IMAGE                       COMMAND                  CREATED
d29a393816ce   redis:7.0                   "docker-entrypoint.s…"   3 seconds ago
e878f0b8f4bc   testcontainers/ryuk:0.14.0   "/bin/ryuk"             3 seconds ago
```

!!! note

    TUnit sets injected properties before it initializes an instance. A fixture can therefore declare its own `ClassDataSource<TFixture>` properties (for example, a shared network or a dependent container) and use them inside `Configure()`. TUnit resolves the dependency graph, initializes the fixtures depth-first, and disposes of them in reverse order.

## Testing ADO.NET services

In addition to the two mentioned base classes, the package contains two more classes: `DbContainerTest` and `DbContainerFixture`, which behave identically but offer additional convenient features when working with services accessible through an ADO.NET provider.

Inherit from either the `DbContainerTest` or `DbContainerFixture` class and override the `Configure()` method to configure your database service.

In this example, we use the default configuration of the PostgreSQL module. The container image capabilities are used to instantiate the database, schema, and test data. During startup, the PostgreSQL container runs SQL scripts placed under the `/docker-entrypoint-initdb.d/` directory automatically.

=== "Configure PostgreSQL Container"
```csharp
--8<-- "tests/Testcontainers.TUnit.Tests/PostgreSqlContainer.cs:ConfigurePostgreSqlContainer"
```

Inheriting from the database container test or fixture class requires you to implement the abstract `DbProviderFactory` property and resolve a compatible `DbProviderFactory` according to your ADO.NET service.

=== "Configure DbProviderFactory"
```csharp
--8<-- "tests/Testcontainers.TUnit.Tests/PostgreSqlContainer.cs:ConfigureDbProviderFactory"
```

!!! note

    Depending on how you initialize and access the database, it may be necessary to override the `ConnectionString` property and replace the default database name with the one actual in use.

After configuring the dependent ADO.NET service, you can add the necessary tests. In this case, we run an SQL `SELECT` statement to retrieve the first record from the `album` table. TUnit injects the test's `CancellationToken` when the test method declares a parameter of that type.

=== "Run Tests"
```csharp
--8<-- "tests/Testcontainers.TUnit.Tests/PostgreSqlContainer.cs:RunTests"
```

--8<-- "docs/modules/_call_out_test_projects.txt"
