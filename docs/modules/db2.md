# IBM DB2

[IBM DB2](https://www.ibm.com/db2), is a relational database engine developed by IBM. The following example provides .NET developers with a starting point to use a IBM DB2 instance in the [xUnit][xunit] tests.

The following example (for windows) uses the following NuGet packages:

```console title="Install the NuGet dependencies"
dotnet add package Testcontainers.Db2
dotnet add package Net.IBM.Data.Db2
dotnet add package xunit
```

Please note: For linux there are currently some hurdles and the package Net.IBM.Data.Db2-lnx has to be used with the following environment variables being set:

  - LD_LIBRARY_PATH
  - PATH
  - DB2_CLI_DRIVER_INSTALL_PATH

One way to achieve this within a test project is to extend the .csproj with a task that writes a .runsettings file. An example is given below:

=== "Example"
    ```xml
    --8<-- "tests/Testcontainers.Db2.Tests/Testcontainers.Db2.Tests.csproj:RunSettingsGeneration"
    ```

IDEs and editors may also require the following packages to run tests: `xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk`.

Copy and paste the following code into a new `.cs` test file within an existing test project.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Db2.Tests/Db2ContainerTest.cs:UseDb2Container"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

## A Note To Developers

Once Testcontainers creates a server instance, developers may use the connection string with any of the popular data-access technologies found in the .NET Ecosystem. Some of these libraries include [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore), [Dapper](https://www.nuget.org/packages/Dapper), and [NHibernate](https://www.nuget.org/packages/NHibernate). At which point, developers can execute database migrations and SQL scripts.

[xunit]: https://xunit.net/
