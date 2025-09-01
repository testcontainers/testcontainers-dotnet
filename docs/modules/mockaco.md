# Mockaco

[Mockaco](https://natenho.github.io/Mockaco/) is a HTTP-based API mock server for .NET Core applications. It's designed to be simple, lightweight, and easy to use for testing and development purposes.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Mockaco
```

You can start a Mockaco container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Test class"
    ```csharp
    public sealed class MockacoContainerTest : IAsyncLifetime
    {
        private readonly MockacoContainer _mockacoContainer = new MockacoBuilder()
            .WithTemplatesPath("./templates")
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
        var baseUrl = new UriBuilder(Uri.UriSchemeHttp, _mockacoContainer.Hostname, 
            _mockacoContainer.GetMappedPublicPort(MockacoBuilder.MockacoPort)).Uri;

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
        var baseUrl = new UriBuilder(Uri.UriSchemeHttp, _mockacoContainer.Hostname, 
            _mockacoContainer.GetMappedPublicPort(MockacoBuilder.MockacoPort)).Uri;

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
