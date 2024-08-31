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

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <Target Condition="$([MSBuild]::IsOSPlatform('Linux'))" Name="TestPrepare" BeforeTargets="RunTests;VSTest">
      <XmlPoke
        XmlInputPath="linux.runsettings"
        Value="$(MSBuildProjectDirectory)/$(OutputPath)clidriver/lib:$(MSBuildProjectDirectory)/$(OutputPath)clidriver/lib/icc"
        Query="/RunSettings/RunConfiguration/EnvironmentVariables/LD_LIBRARY_PATH"
      />
      <XmlPoke
        XmlInputPath="linux.runsettings"
        Value="$(MSBuildProjectDirectory)/$(OutputPath)clidriver/bin"
        Query="/RunSettings/RunConfiguration/EnvironmentVariables/PATH"
      />
      <XmlPoke
        XmlInputPath="linux.runsettings"
        Value="$(MSBuildProjectDirectory)/$(OutputPath)clidriver"
        Query="/RunSettings/RunConfiguration/EnvironmentVariables/DB2_CLI_DRIVER_INSTALL_PATH"
      />
    </Target>
    <PropertyGroup>
        <TargetFrameworks>net8.0</TargetFrameworks>
        <IsPackable>false</IsPackable>
        <IsPublishable>false</IsPublishable>
        <ProcessorArchitecture>amd64</ProcessorArchitecture>
        <RunSettingsFilePath Condition="$([MSBuild]::IsOSPlatform('Linux'))">$(MSBuildProjectDirectory)/linux.runsettings</RunSettingsFilePath>
    </PropertyGroup>
    <ItemGroup>
        <PackageReference Include="Microsoft.NET.Test.Sdk" />
        <PackageReference Include="coverlet.collector" />
        <PackageReference Include="Net.IBM.Data.Db2" Condition="$([MSBuild]::IsOSPlatform('Windows'))" />
        <PackageReference Include="Net.IBM.Data.Db2-lnx" Condition="$([MSBuild]::IsOSPlatform('Linux'))" />
        <PackageReference Include="xunit.runner.visualstudio" />
        <PackageReference Include="xunit" />
    </ItemGroup>
    <ItemGroup>
        <ProjectReference Include="../../src/Testcontainers.Db2/Testcontainers.Db2.csproj" />
        <ProjectReference Include="../Testcontainers.Commons/Testcontainers.Commons.csproj" />
        <ProjectReference Include="..\Testcontainers.Platform.Windows.Tests\Testcontainers.Platform.Windows.Tests.csproj" />
    </ItemGroup>
</Project>
```

IDEs and editors may also require the following packages to run tests: `xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk`.

Copy and paste the following code into a new `.cs` test file within an existing test project.

```csharp
using IBM.Data.Db2;

namespace Testcontainers.Db2;

public sealed class Db2ContainerTest : IAsyncLifetime
{
  private readonly Db2Container _db2Container = new Db2Builder().Build();

  public Task InitializeAsync()
  {
    return _db2Container.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _db2Container.DisposeAsync().AsTask();
  }

  [Fact]
  public async Task ReadFromDb2Database()
  {
    // Given
    using DbConnection connection = new DB2Connection(_db2Container.GetConnectionString());

    // When
    connection.Open();

    using DbCommand command = connection.CreateCommand();
    command.CommandText = "SELECT 1 FROM SYSIBM.SYSDUMMY1;";

    var actual = await command.ExecuteScalarAsync() as int?;
    Assert.Equal(1, actual.GetValueOrDefault());

    // Then
    Assert.Equal(ConnectionState.Open, connection.State);
  }
}
```

To execute the tests, use the command `dotnet test` from a terminal.

## A Note To Developers

Once Testcontainers creates a server instance, developers may use the connection string with any of the popular data-access technologies found in the .NET Ecosystem. Some of these libraries include [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore), [Dapper](https://www.nuget.org/packages/Dapper), and [NHibernate](https://www.nuget.org/packages/NHibernate). At which point, developers can execute database migrations and SQL scripts.

[xunit]: https://xunit.net/
