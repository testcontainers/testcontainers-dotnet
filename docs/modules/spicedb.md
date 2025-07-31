# SpiceDB

[SpiceDB](https://authzed.com/spicedb/) is an open-source, Google Zanzibar-inspired permissions database that provides a centralized service to store, compute, and validate application permissions. It enables fine-grained authorization at scale with a flexible relationship-based permission model.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.SpiceDB
```

You can start a SpiceDB container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"

````csharp
public sealed class SpiceDBContainerTest : IAsyncLifetime
{
private readonly SpiceDBContainer \_spicedbContainer = new SpiceDBBuilder().Build();

        public Task InitializeAsync()
        {
            return _spicedbContainer.StartAsync();
        }

        public Task DisposeAsync()
        {
            return _spicedbContainer.DisposeAsync().AsTask();
        }

        [Fact]
        public async Task SpiceDBContainer_IsRunning_ReturnsTrue()
        {
            // Given
            var containerState = await _spicedbContainer.GetStateAsync();

            // ...
        }
    }
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
`xml
    <ItemGroup>
      <PackageReference Include="Microsoft.NET.Test.Sdk" />
      <PackageReference Include="xunit" />
      <PackageReference Include="xunit.runner.visualstudio" />
      <PackageReference Include="Testcontainers.SpiceDB" />
    </ItemGroup>
    `

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/\_call_out_test_projects.txt"
````
