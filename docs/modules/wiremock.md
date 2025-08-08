# WireMock

[WireMock](https://wiremock.org/) is a flexible library for stubbing and mocking web services. It allows you to simulate HTTP-based APIs for testing purposes by creating mock servers that can return predefined responses based on request matching rules.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.WireMock
```

You can start a WireMock container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.WireMock.Tests/WireMockContainerTest.cs:UseWireMockContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.WireMock.Tests/Testcontainers.WireMock.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

## Advanced Configuration

### Adding Mappings

WireMock allows you to configure request/response mappings in several ways:

#### Static Mapping Files

You can add mapping files when building the container:

```csharp
var container = new WireMockBuilder()
    .WithMapping("/path/to/mapping.json")
    .Build();
```

#### Dynamic Mappings

You can add mappings at runtime:

```csharp
const string mappingJson = @"{
    ""request"": {
        ""method"": ""GET"",
        ""url"": ""/api/users""
    },
    ""response"": {
        ""status"": 200,
        ""jsonBody"": {
            ""users"": []
        }
    }
}";

await container.AddMappingFromJsonAsync(mappingJson);
```

### Static Files

WireMock can serve static files for stubbed responses:

```csharp
var container = new WireMockBuilder()
    .WithStaticFile("/path/to/response.json")
    .Build();
```

Or add them dynamically:

```csharp
await container.AddStaticFileAsync("response.json", fileContent);
```

### Extensions

WireMock supports extensions for advanced functionality:

```csharp
var container = new WireMockBuilder()
    .WithExtension("com.example.MyExtension")
    .Build();
```

### CLI Arguments

You can pass custom command-line arguments to WireMock:

```csharp
var container = new WireMockBuilder()
    .WithCliArg("--verbose")
    .WithCliArg("--root-dir=/custom")
    .WithoutBanner() // Disable the startup banner
    .Build();
```

### Admin API

The WireMock container provides access to the admin API:

```csharp
// Get the base URL for making requests
var baseUrl = container.GetBaseUrl();

// Get the admin URL for management operations
var adminUrl = container.GetAdminUrl();

// Reset all mappings and requests
await container.ResetAsync();

// Get request count
var result = await container.GetRequestCountAsync();

// Get all requests
var requests = await container.GetRequestsAsync();
```

## Common Use Cases

### API Testing

WireMock is ideal for testing applications that depend on external APIs:

```csharp
[Fact]
public async Task Should_Handle_External_Api_Response()
{
    // Arrange
    var wireMockContainer = new WireMockBuilder().Build();
    await wireMockContainer.StartAsync();

    const string mapping = @"{
        ""request"": { ""method"": ""GET"", ""url"": ""/api/data"" },
        ""response"": { ""status"": 200, ""body"": ""Success"" }
    }";
    
    await wireMockContainer.AddMappingFromJsonAsync(mapping);

    // Act - test your application using wireMockContainer.GetBaseUrl()
    
    // Assert - verify your application behavior
}
```

### Contract Testing

Use WireMock to verify that your application correctly handles various API responses:

```csharp
[Theory]
[InlineData(200, "Success")]
[InlineData(404, "Not Found")]
[InlineData(500, "Server Error")]
public async Task Should_Handle_Different_Status_Codes(int statusCode, string message)
{
    // Configure WireMock to return different status codes
    // Test your application's response handling
}
```

### Performance Testing

WireMock can simulate slow responses for performance testing:

```csharp
const string slowMapping = @"{
    ""request"": { ""method"": ""GET"", ""url"": ""/slow"" },
    ""response"": { 
        ""status"": 200,
        ""body"": ""Delayed response"",
        ""fixedDelayMilliseconds"": 5000
    }
}";
```

--8<-- "docs/modules/_call_out_test_projects.txt"