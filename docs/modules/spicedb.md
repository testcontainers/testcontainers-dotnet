# SpiceDB

[SpiceDB](https://authzed.com/spicedb/) is an open-source, Google Zanzibar-inspired permissions database that provides a centralized service to store, compute, and validate application permissions. It enables fine-grained authorization at scale with a flexible relationship-based permission model.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.SpiceDB
```

You can start a SpiceDB container instance from any .NET application. To create and start a container instance with the default configuration, use the module-specific builder as shown below:

=== "Start a SpiceDB container"
`csharp
    var spicedbContainer = new SpiceDBBuilder().Build();
    await spicedbContainer.StartAsync();
    `

The following example utilizes the [xUnit.net](/test_frameworks/xunit_net/) module to reduce overhead by automatically managing the lifecycle of the dependent container instance. It creates and starts the container using the module-specific builder and injects it as a shared class fixture into the test class.

=== "Usage Example"
```csharp
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

            // When & Then
            Assert.Equal(ResourceState.Running, containerState.Status);
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

## Configuration

The SpiceDB container is pre-configured with the following settings:

- **Image**: `authzed/spicedb:v1.45.1`
- **Port**: `50051` (gRPC port)
- **Wait Strategy**: Waits for the SpiceDB CLI ping command to complete successfully

## Connection

SpiceDB exposes a gRPC API on port 50051. You can connect to it using the container's host and port:

```csharp
var host = spicedbContainer.Hostname;
var port = spicedbContainer.GetMappedPublicPort(50051);
// Connect to gRPC endpoint at host:port
```

## Features

- **Relationship-based permissions**: Define complex permission models using relationships between resources
- **Consistency guarantees**: Provides ACID transactions for permission checks
- **Schema management**: Define and evolve permission schemas
- **gRPC API**: High-performance API for permission operations
- **Docker-based**: Easy to run and test in containerized environments
