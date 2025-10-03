# Mockaco

[Mockaco](https://natenho.github.io/Mockaco/) is a HTTP-based API mock server for .NET Core applications. It's designed to be simple, lightweight, and easy to use for testing and development purposes.

## Prerequisites

Before using Mockaco, you need to create mock templates (JSON files) that define the API endpoints and their responses. These templates should be placed in a folder that will be mounted to the container.

Create a template file (e.g., `ping-pong.json`) in your templates folder:

```json title="./templates/ping-pong.json"
{
  "request": {
    "method": "GET",
    "route": "ping"
  },
  "response": {
    "status": "OK",
    "body": {
      "response": "pong"
    }
  }
}
```

For more information about creating templates, see the [official Mockaco documentation](https://natenho.github.io/Mockaco/docs/quick-start/create-mock).

## Installation

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Mockaco
```

You can start a Mockaco container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

**Note:** The `WithTemplatesPath()` method specifies the local folder containing your JSON template files, which will be mounted to the container's `/app/Mocks` directory.

=== "Test class"
    ```csharp
    public sealed class MockacoContainerTest : IAsyncLifetime
    {
        private readonly MockacoContainer _mockacoContainer = new MockacoBuilder()
            .WithTemplatesPath("./templates") // Local folder with JSON templates
            .Build();

        public async ValueTask InitializeAsync()
        {
            await _mockacoContainer.StartAsync()
                .ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await _mockacoContainer.DisposeAsync()
                .ConfigureAwait(false);
        }
    }
    ```

Set up and call a mock endpoint:

=== "Mock endpoint setup"
    ```csharp
    [Fact]
    public async Task SetupAndCallMockEndpoint()
    {
        // Given
        using var httpClient = new HttpClient();
        var baseUrl = new Uri(_mockacoContainer.GetBaseAddress());

        // When - Call the mock endpoint
        var response = await httpClient.GetAsync(new Uri(baseUrl, "/ping"));

        // Then
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("pong", content);
    }
    ```

Verify API calls using the verification endpoint:

=== "Verify API calls"
    ```csharp
    [Fact]
    public async Task VerifyApiCallsWithVerificationEndpoint()
    {
        // Given
        using var httpClient = new HttpClient();
        var baseUrl = new Uri(_mockacoContainer.GetBaseAddress());

        // When - Call an endpoint and then verify
        await httpClient.GetAsync(new Uri(baseUrl, "/ping"));
        var verification = await _mockacoContainer.GetVerifyAsync("/ping");

        // Then
        Assert.NotNull(verification);
        Assert.Equal("/ping", verification.Route);
        Assert.Contains("pong", verification.Body);
    }
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    <PackageReference Include="Microsoft.NET.Test.Sdk"/>
    <PackageReference Include="xunit.v3"/>
    <PackageReference Include="xunit.runner.visualstudio"/>
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"
