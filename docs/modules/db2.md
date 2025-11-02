# Db2

[Db2](https://www.ibm.com/db2) is a relational database engine developed by IBM.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Db2
```

!!! warning

    The Linux client dependency, [Net.IBM.Data.Db2-lnx](https://www.nuget.org/packages/Net.IBM.Data.Db2-lnx), requires additional configurations. We use the [Testcontainers.Db2.Tests.targets](https://github.com/testcontainers/testcontainers-dotnet/blob/develop/tests/Testcontainers.Db2.Tests/Testcontainers.Db2.Tests.targets) file to configure the environment variables: `LD_LIBRARY_PATH`, `PATH`, `DB2_CLI_DRIVER_INSTALL_PATH`, at runtime.

You can start a Db2 container instance from any .NET application. To create and start a container instance with the default configuration, use the module-specific builder as shown below:

=== "Start a Db2 container"
    ```csharp
    var db2Container = new Db2Builder().Build();
    await db2Container.StartAsync();
    ```

The following example utilizes the [xUnit.net](/test_frameworks/xunit_net/) module to reduce overhead by automatically managing the lifecycle of the dependent container instance. It creates and starts the container using the module-specific builder and injects it as a shared class fixture into the test class.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Db2.Tests/Db2ContainerTest.cs:UseDb2Container"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Db2.Tests/Testcontainers.Db2.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"