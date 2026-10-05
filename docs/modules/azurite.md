# Azurite

[Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) is an open-source emulator for Azure Blob, Queue and Table storage that provides a local environment for developing and testing Azure Storage applications.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.Azurite
```

You can start an Azurite container instance from any .NET application. To create and start a container instance with the default configuration, use the module-specific builder as shown below:

=== "Start an Azurite Container"
    ```csharp
    var azuriteContainer = new AzuriteBuilder("mcr.microsoft.com/azure-storage/azurite:3.36.0").Build();
    await azuriteContainer.StartAsync();
    ```

The following example connects to the Blob, Queue and Table services using the container's connection string:

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

## SSL/HTTPS

Use `WithSsl` to enable HTTPS and map the server certificate and private key (PEM) into the container. When SSL is enabled, `GetConnectionString()` returns `DefaultEndpointsProtocol=https` and `https` Blob, Queue and Table endpoints.

```csharp
--8<-- "tests/Testcontainers.Azurite.Tests/AzuriteContainerTest.cs:AzuriteSslBuilder"
```

!!! note

    The client must trust the server certificate. The following example validates the certificate against the CA certificate that signed it and assigns the transport to the client options.

```csharp
--8<-- "tests/Testcontainers.Azurite.Tests/AzuriteContainerTest.cs:AzuriteSslClientTransport"
```

```csharp
--8<-- "tests/Testcontainers.Azurite.Tests/AzuriteContainerTest.cs:AzuriteSslClientOptions"
```
