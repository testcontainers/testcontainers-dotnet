# Apache Cassandra

[Apache Cassandra](https://cassandra.apache.org/) is a powerful, open-source, distributed NoSQL database that is highly available and fault-tolerant, used to store, manage, and retrieve structured data.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Cassandra
```

You can start an Apache Cassandra container instance from any .NET application. To create and start a container instance with the default configuration, use the module-specific builder as shown below:

=== "Start a Cassandra container"
    ```csharp
    var cassandraContainer = new CassandraBuilder().Build();
    await cassandraContainer.StartAsync();
    ```

The following example utilizes the [xUnit.net](/test_frameworks/xunit_net/) module to reduce overhead by automatically managing the lifecycle of the dependent container instance. It creates and starts the container using the module-specific builder and injects it as a shared class fixture into the test class.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Cassandra.Tests/CassandraContainerTest.cs:UseCassandraContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Cassandra.Tests/Testcontainers.Cassandra.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"