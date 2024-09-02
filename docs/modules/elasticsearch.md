# Elasticsearch

[Elasticsearch](https://www.elastic.co/elasticsearch/) is a distributed, RESTful search and analytics engine capable of addressing a growing number of use cases. As the heart of the Elastic Stack, it centrally stores data for lightning fast search, fine‑tuned relevancy, and powerful analytics that scale with ease.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Elasticsearch
```

You can start an Elasticsearch container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Elasticsearch.Tests/ElasticsearchContainerTest.cs:UseElasticsearchContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Elasticsearch.Tests/Testcontainers.Elasticsearch.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## A Note To Developers

The Testcontainers module creates a container that listens to requests over **HTTPS**. To communicate with the Elasticsearch instance, developers must create a `ElasticsearchClientSettings` instance and set the `ServerCertificateValidationCallback` delegate to `CertificateValidations.AllowAll`. Failing to do so will result in a communication failure as the .NET will reject the certificate coming from the container.