# FlociAz

[FlociAz](https://github.com/floci-io/floci-az) emulates Azure management and data-plane APIs in one container. The module starts in a Docker-safe mode: services that would otherwise create child containers use their mocked or embedded implementations.

Add the module to a test project:

```shell
dotnet add package Testcontainers.FlociAz
```

Start FlociAz and use its storage connection string or service-specific endpoints:

```csharp
await using var flociAz = new FlociAzBuilder("floci/floci-az:0.12.0")
    .Build();

await flociAz.StartAsync();

var blobs = new BlobServiceClient(flociAz.GetConnectionString());
var keyVaultEndpoint = flociAz.GetServiceEndpoint("keyvault");
var armEndpoint = flociAz.GetEndpoint();
```

## Service compatibility

The following matrix is covered against FlociAz 0.12.0. “Real” means the test reaches the service's actual protocol or runtime, not only its ARM representation.

| Service | Verified compatibility |
|---------|------------------------|
| Blob Storage | Azure Storage SDK create, upload, and download |
| Queue Storage | Azure Storage SDK create, send, and receive |
| Table Storage | Azure Data Tables SDK create, insert, and read |
| Functions | Management lifecycle, mocked invocation, and real Node.js runtime execution |
| App Configuration | Key-value write and read |
| Cosmos DB for NoSQL | Database, container, and partitioned document lifecycle |
| Key Vault | Authenticated secret write and read |
| Event Hubs | Mocked namespace management only |
| Azure SQL Database | ARM server lifecycle in the default management-only provider |
| Azure Database for PostgreSQL | ARM lifecycle and real Npgsql query |
| Service Bus | Mocked queue/topic/subscription/rule topology and real Azure SDK AMQP send/receive |
| Azure Monitor | Workspace, collection endpoint/rule, log ingestion, and KQL query |
| AKS | Mocked ARM cluster lifecycle |
| Azure Container Instances | Mocked ARM container-group lifecycle |
| Virtual Machines | Mocked ARM VM lifecycle |
| API Management | ARM service lifecycle |
| Azure Cache for Redis | ARM lifecycle and real RESP write/read |
| Azure Container Registry | ARM lifecycle and real Registry V2 API |
| Microsoft Entra ID | OAuth client-credentials token issuance |
| Microsoft Graph | Service-principal discovery and seeded group membership |
| Communication Services Email | Send operation and inspection mailbox |
| Azure Resource Manager | Resource-group and service resource lifecycle |
| Virtual Network | ARM virtual-network lifecycle |
| Event Grid | Topic keys, event publication, and lifecycle |
| Managed Identity | User-assigned ARM lifecycle and IMDS token issuance |

### Upstream 0.12.0 boundaries

- Event Hubs AMQP is deliberately hard-coded to mocked mode upstream because Azure SDK connections reset.
- Azure Container Instances accepts `mocked=false`, but 0.12.0 still behaves as mocked mode; container-backed mode is planned upstream.
- AKS real mode starts k3s, but does not reliably transition the ARM resource from `Creating` to `Succeeded` in the containerized Testcontainers topology. The module therefore defaults it to mocked mode.
- Azure SQL's managed data plane requires explicit acceptance of the Microsoft SQL Server EULA. The module never accepts it on the user's behalf; enable and test that mode only after reviewing the license.

## Docker-backed services

Functions, PostgreSQL, Service Bus, Redis, and ACR have verified real modes. Grant FlociAz Docker access and opt individual services into real mode:

```csharp
await using var flociAz = new FlociAzBuilder("floci/floci-az:0.12.0")
    .WithDockerSocket()
    .WithEnvironment("FLOCI_AZ_SERVICES_FUNCTIONS_MOCKED", "false")
    .WithEnvironment("FLOCI_AZ_SERVICES_POSTGRES_MOCKED", "false")
    .WithEnvironment("FLOCI_AZ_SERVICES_SERVICE_BUS_MOCKED", "false")
    .WithEnvironment("FLOCI_AZ_SERVICES_REDIS_MOCKED", "false")
    .WithEnvironment("FLOCI_AZ_SERVICES_ACR_MOCKED", "false")
    .Build();
```

!!! warning

    The Docker socket provides root-equivalent access to the Docker host. Use `WithDockerSocket()` only with trusted images. Child containers and volumes are namespaced and registered with the Testcontainers Resource Reaper.

FlociAz `/connect` responses contain the child container's internal hostname and port. Resolve that pair to a host port before connecting from the test process:

```csharp
var mappedPort = await flociAz.GetSidecarMappedPublicPortAsync(
    sidecarHostname,
    sidecarPrivatePort);
```

Use `flociAz.Hostname` with the returned port. This works with local and remote Docker endpoints supported by Testcontainers.
