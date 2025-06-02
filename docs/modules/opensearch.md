# OpenSearch

[OpenSearch](https://opensearch.org/) is an open-source, enterprise-grade search and observability suite that brings order to unstructured data at scale.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.OpenSearch
```

You can start an OpenSearch container instance from any .NET application. To create and start a container instance with the default configuration, use the module-specific builder as shown below:

=== "Start an OpenSearch container"
    ```csharp
    var openSearchContainer = new OpenSearchBuilder().Build();
    await openSearchContainer.StartAsync();
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Base test class"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:BaseClass"
    }
    ```
=== "SslBasicAuth"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:SslBasicAuth"
    ```
=== "SslBasicAuth CustomPassword"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:SslBasicAuthCustomPassword"
    ```
=== "InsecureNoAuth"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:InsecureNoAuth"
    ```

How to check that client established connection:
=== "PingExample"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:PingExample"
    ```

Creating index and index alias:
=== "IndexAndAliasCreation"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:IndexAndAliasCreation"
    ```
=== "CreateTestIndexImpl"
    ```csharp
    --8<-- "tests/Testcontainers.OpenSearch.Tests/OpenSearchContainerTest.cs:CreateTestIndexImpl"
    ```

Indexing and searching for document:
=== "IndexingDocument"
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