# OpenSearch

[OpenSearch](https://opensearch.org/) is an open-source, enterprise-grade search and observability suite that brings order to unstructured data at scale.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.OpenSearch
```

You can start an OpenSearch container instance from any .NET application. To create and start a container instance with the default configuration, use the module-specific builder as shown below:

=== "Start an OpenSearch container"
    ```csharp
    var openSearchContainer = new OpenSearchBuilder("opensearchproject/opensearch:2.12.0").Build();
    await openSearchContainer.StartAsync();
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Base test class"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:BaseClass"
    }
    ```
=== "Insecure no auth"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:InsecureNoAuth"
    ```
=== "SSL default credentials"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:SslBasicAuthDefaultCredentials"
    ```
=== "SSL custom credentials"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:SslBasicAuthCustomCredentials"
    ```

How to check if the client has established a connection:

=== "Ping example"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:PingExample"
    ```

Creating an index and alias:

=== "Create index and alias"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:CreateIndexAndAlias"
    ```
=== "Create index implementation"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:CreateIndexImplementation"
    ```

Indexing and searching a document:

=== "Indexing document"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:IndexingDocument"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.OpenSearch.Tests/Testcontainers.OpenSearch.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"