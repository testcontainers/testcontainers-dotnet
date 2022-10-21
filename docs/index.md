# Testcontainers

![Testcontainers Banner](Banner.png)

## About

Testcontainers-Dotnet is a library to support tests with throwaway instances of Docker containers for all compatible .NET Standard versions. The library is built on top of the .NET Docker remote API and provides a lightweight implementation to support your test environment in all circumstances.

Choose from existing pre-configured configurations and start containers within a second, to support and run your tests. Or create your own containers with Dockerfiles and run your tests immediately afterward.

## Supported operating systems

Testcontainers supports Windows, Linux, and macOS as host systems. Linux Docker containers are supported on all three operating systems.

Native Windows Docker containers are only supported on Windows. Windows requires the host operating system version to match the container operating system version. You'll find further information about Windows container version compatibility [here](https://docs.microsoft.com/en-us/virtualization/windowscontainers/deploy-containers/version-compatibility).

Keep in mind to enable the correct Docker engine on Windows host systems to match the container operating system. With Docker Desktop you can switch the engine either with the tray icon context menu or: `$env:ProgramFiles\Docker\Docker\DockerCli.exe -SwitchDaemon` or `-SwitchLinuxEngine`, `-SwitchWindowsEngine`.

## Pre-configured containers

The pre-configured Testcontainers below are supported. Further examples can be found in [TestcontainersContainerTest][1] as well as in [database][2] or [message broker][3] tests.

- Apache CouchDB (couchdb:2.3.1)
- Azurite (mcr.microsoft.com/azure-storage/azurite:3.18.0)
- Cosmos DB Linux Emulator (mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest)
- Couchbase (couchbase:6.5.1)
- Elasticsearch (elasticsearch:8.3.2)
- LocalStack (localstack/localstack:1.2.0)
- MariaDB (mariadb:10.8)
- Microsoft SQL Server (mcr.microsoft.com/mssql/server:2017-CU28-ubuntu-16.04)
- MongoDB (mongo:5.0.6)
- MySQL (mysql:8.0.28)
- Neo4j (neo4j:4.4.11)
- Oracle Database (gvenzl/oracle-xe:21-slim)
- PostgreSQL (postgres:11.14)
- Redis (redis:5.0.14)
- Apache Kafka (confluentinc/cp-kafka:6.0.5)
- RabbitMQ (rabbitmq:3.7.28)

## License

See [LICENSE](https://raw.githubusercontent.com/testcontainers/testcontainers-dotnet/master/LICENSE).

## Copyright

Copyright (c) 2019 - 2022 Andre Hofmeister and other authors.

----

Join our [Slack workspace][739e5ee9] | [Testcontainers OSS][432332aa] | [Testcontainers Cloud][7b2badd4]

[739e5ee9]: https://slack.testcontainers.org/
[432332aa]: https://www.testcontainers.org/
[7b2badd4]: https://www.testcontainers.cloud/
