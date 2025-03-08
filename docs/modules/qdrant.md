# Qdrant

[Qdrant](https://qdrant.tech/) is an open source vector database designed for scalable and efficient similarity search
and nearest neighbor retrieval. It provides a RESTful API and gRPC support, making it easy to integrate with various
applications, including search, recommendation, AI, and machine learning systems.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Qdrant
```

You can start a Qdrant container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Qdrant.Tests/QdrantContainerTest.cs:UseQdrantContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Qdrant.Tests/Testcontainers.Qdrant.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## A Note To Developers

The Testcontainers module creates a container that listens to requests over **HTTP**. The official Qdrant client uses the gRPC APIs to communicate
with Qdrant. **.NET Core** and **.NET** support the above example with no additional configuration. However, **.NET Framework** has limited supported for gRPC over HTTP/2, but it can be enabled by

- Configuring the Testcontainers module to use TLS and using **HTTPS** to communicate with the cluster
- Configuring Server certificate validation
- Reference `System.Net.Http.WinHttpHandler` 6.0.1 or later, and configuring `WinHttpHandler` as the inner handler for `GrpcChannelOptions` on the Qdrant client

Refer to the [official Qdrant .NET SDK](https://github.com/qdrant/qdrant-dotnet) for further details.

## Server certificate validation and API key

The following example creates a self-signed certificate and configures the Testcontainers module to use TLS with the certificate and private key, along with an API key for authentication. The Qdrant client is configured to validate the TLS certificate using its thumbprint, and use the API key
to authenticate.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Qdrant.Tests/QdrantContainerApiKeyCertificateTest.cs:UseQdrantContainer"
    ```
