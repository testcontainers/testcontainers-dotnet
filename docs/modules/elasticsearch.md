# Elasticsearch

[Elasticsearch](https://www.elastic.co/elasticsearch/) is a distributed, RESTful search and analytics engine capable of addressing a growing number of use cases. As the heart of the Elastic Stack, it centrally stores data for lightning fast search, fine‑tuned relevancy, and powerful analytics that scale with ease.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Elasticsearch
```

You can start an Elasticsearch container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.Elasticsearch.Tests/ElasticsearchContainerTest.cs:CreateElasticsearchContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

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

## OpenTelemetry (OTLP)

Elasticsearch 9.5 and later accept OTLP over HTTP. `ElasticsearchContainer.GetOtlpEndpoint()` returns the base endpoint the exporter sends the telemetry data to. If the endpoint is set via `OTEL_EXPORTER_OTLP_ENDPOINT`, the exporter appends the signal path, such as `v1/traces`. If the endpoint is set via `OtlpExporterOptions.Endpoint`, the exporter uses it as is, and the signal path must be appended manually. In contrast to the connection string, the endpoint does not contain the credentials. Clients must send them in the `Authorization` header:

=== "Export Telemetry Data"
    ```csharp
    --8<-- "tests/Testcontainers.Elasticsearch.Tests/ElasticsearchContainerOtlpTest.cs:UseElasticsearchOtlpEndpoint"
    ```

## A Note To Developers

The Testcontainers module creates a container that listens to requests over **HTTPS**. Elasticsearch generates a self-signed certificate authority (CA) during the startup that signs the HTTP certificate. `ElasticsearchContainer.GetCertificateAsync()` reads this certificate authority (CA) from the container. Configure the client to trust it, otherwise .NET will reject the certificate coming from the container.

Besides the Elasticsearch client, any other client can be configured to trust the certificate authority (CA) too. The example below uses the `CertificateValidations.AuthorityIsRoot(X509Certificate)` helper from `Elastic.Transport`. Without it, build an `X509Chain` with `X509ChainTrustMode.CustomRootTrust` and add the certificate authority (CA) to `ChainPolicy.CustomTrustStore`:

=== "Trust The Certificate Authority"
    ```csharp
    --8<-- "tests/Testcontainers.Elasticsearch.Tests/ElasticsearchContainerTest.cs:UseElasticsearchCertificate"
    ```