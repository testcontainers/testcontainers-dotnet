# Modules

Modules are a great example of the capabilities of Testcontainers for .NET. Choose one of the pre-configurations below or find further examples in [TestcontainersContainerTest][testcontainers-container-tests] as well as in the [database][testcontainers-database-tests] or [message broker][testcontainers-message-broker-tests] tests to set up your test environment.

!!!warning

    We are redesigning modules and removing the extension method in the future. Modules will become independent projects that allow more complex and advanced features. Due to an old design flaw, we cannot distinguish between a generic and module builder. If you rely on a module you will get an obsolete warning until the next version of Testcontainers gets released. You will find more information [here](https://github.com/testcontainers/testcontainers-dotnet/issues/750#issuecomment-1412257694).

| Module                     | Container image                                                  |
|----------------------------|------------------------------------------------------------------|
| LocalStack                 | `localstack/localstack:1.2.0`                                    |
| Apache CouchDB             | `couchdb:2.3.1`                                                  |
| Azurite                    | `mcr.microsoft.com/azure-storage/azurite:3.18.0`                 |
| Cosmos DB Linux Emulator   | `mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest`  |
| Couchbase                  | `couchbase:6.5.1`                                                |
| Elasticsearch              | `elasticsearch:8.3.2`                                            |
| MariaDB                    | `mariadb:10.8`                                                   |
| Microsoft SQL Server       | `mcr.microsoft.com/mssql/server:2017-CU28-ubuntu-16.04`          |
| MongoDB                    | `mongo:5.0.6`                                                    |
| MySQL                      | `mysql:8.0.28`                                                   |
| Neo4j                      | `neo4j:4.4.11`                                                   |
| Oracle Database            | `gvenzl/oracle-xe:21-slim`                                       |
| PostgreSQL                 | `postgres:11.14`                                                 |
| Redis                      | `redis:5.0.14`                                                   |
| Apache Kafka               | `confluentinc/cp-kafka:6.0.5`                                    |
| RabbitMQ                   | `rabbitmq:3.7.28`                                                |

Due to a design flaw in the current module system, pre-configured containers must be configured through their corresponding extension method (`WithDatabase` or `WithMessageBroker`):

```csharp
await new ContainerBuilder<PostgreSqlTestcontainer>()
  .WithDatabase(new PostgreSqlTestcontainerConfiguration())
  .Build()
  .StartAsync()
  .ConfigureAwait(false);
```

[testcontainers-container-tests]: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/tests/Testcontainers.Tests/Unit/Containers/Unix/TestcontainersContainerTest.cs
[testcontainers-database-tests]: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/tests/Testcontainers.Tests/Unit/Containers/Unix/Modules/Databases
[testcontainers-message-broker-tests]: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/tests/Testcontainers.Tests/Unit/Containers/Unix/Modules/MessageBrokers
