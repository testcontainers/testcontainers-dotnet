# Azurite

[Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) is an open-source emulator for Azure Blob, Queue and Table storage that provides a local environment for developing and testing Azure Storage applications.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Azurite
```

You can start an Azurite container instance from any .NET application. Here, we create different container instances and pass them to the base test class. This allows us to test different configurations.

=== "Create Container Instance"
    ```csharp
    --8<-- "tests/Testcontainers.Azurite.Tests/AzuriteContainerTest.cs:CreateAzuriteContainer"
    ```

This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.Azurite.Tests/AzuriteContainerTest.cs:UseAzuriteContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.Azurite.Tests/Testcontainers.Azurite.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Access Azurite

```csharp title="Gets the Azurite connection string"
string connectionString = _azuriteContainer.GetConnectionString();
```

```csharp title="Gets the Blob, Queue and Table endpoints"
string blobEndpoint = _azuriteContainer.GetBlobEndpoint();
string queueEndpoint = _azuriteContainer.GetQueueEndpoint();
string tableEndpoint = _azuriteContainer.GetTableEndpoint();
```

## Configure SSL

To enable HTTPS, use the following container builder method to configure Azurite with an SSL certificate and private key. The connection string and the Blob, Queue and Table endpoints then use the `https` scheme:

=== "Configure the SSL Certificate"
    ```csharp
    --8<-- "tests/Testcontainers.Azurite.Tests/AzuriteContainerTest.cs:ConfigureAzuriteContainerCertificate"
    ```

!!! note

    Please ensure that both the certificate and private key are provided in PEM format.

The client must trust the server certificate. In this example, the Azure client validates the SSL certificate against the CA certificate that signed it. The transport is assigned to the client options, as shown in the usage example:

=== "Configure the Azure Client"
    ```csharp
    --8<-- "tests/Testcontainers.Azurite.Tests/AzuriteContainerTest.cs:ConfigureAzuriteClientCertificate"
    ```

## Enable in-memory persistence

By default, Azurite persists its data to disk. If your tests do not need the data after the container stops, use the following configuration to keep it in memory instead:

=== "In-Memory Persistence Configuration"
    ```csharp
    --8<-- "tests/Testcontainers.Azurite.Tests/AzuriteContainerTest.cs:InMemoryContainerConfiguration"
    ```

The in-memory storage is limited to 50% of the total memory of the container by default. To set a different limit, pass the limit in megabytes to `WithInMemoryPersistence`.
