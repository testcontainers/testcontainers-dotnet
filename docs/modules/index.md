# Modules

Modules are great examples of Testcontainers' capabilities. To write tests against real dependencies, you can either choose one of the pre-configurations listed below or create your own implementation.

Modules are standalone dependencies that can be installed from [NuGet.org](https://www.nuget.org/profiles/Testcontainers). To use a module in your test project, you need to add it as a dependency first:

```console
dotnet add package Testcontainers.ModuleName
```

All modules follow the same design and come pre-configured with best practices. Usually, you do not need to worry about configuring them yourself. To create and start a container, all you need is:

```csharp
var moduleNameContainer = new ModuleNameBuilder().Build();
await moduleNameContainer.StartAsync();
```

!!! note

    We will add module-specific documentations soon.

| Module          | Image                                                               | NuGet                                                                | Source                                                                                                          |
|-----------------|---------------------------------------------------------------------|----------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------|
| Azure Cosmos DB | `mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest`     | [NuGet](https://www.nuget.org/packages/Testcontainers.CosmosDb)      | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.CosmosDb)      |
| Azure SQL Edge  | `mcr.microsoft.com/azure-sql-edge:1.0.7`                            | [NuGet](https://www.nuget.org/packages/Testcontainers.SqlEdge)       | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.SqlEdge)       |
| Azurite         | `mcr.microsoft.com/azure-storage/azurite:3.24.0`                    | [NuGet](https://www.nuget.org/packages/Testcontainers.Azurite)       | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Azurite)       |
| Bigtable        | `gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators` | [NuGet](https://www.nuget.org/packages/Testcontainers.Bigtable)      | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Bigtable)      |
| ClickHouse      | `clickhouse/clickhouse-server:23.6-alpine`                          | [NuGet](https://www.nuget.org/packages/Testcontainers.ClickHouse)    | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.ClickHouse)    |
| Consul          | `consul:1.15`                                                       | [NuGet](https://www.nuget.org/packages/Testcontainers.Consul)        | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Consul)        |
| Couchbase       | `couchbase:community-7.0.2`                                         | [NuGet](https://www.nuget.org/packages/Testcontainers.Couchbase)     | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Couchbase)     |
| CouchDB         | `couchdb:3.3`                                                       | [NuGet](https://www.nuget.org/packages/Testcontainers.CouchDb)       | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.CouchDb)       |
| DynamoDB        | `amazon/dynamodb-local:1.21.0`                                      | [NuGet](https://www.nuget.org/packages/Testcontainers.DynamoDb)      | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.DynamoDb)      |
| Elasticsearch   | `elasticsearch:8.6.1`                                               | [NuGet](https://www.nuget.org/packages/Testcontainers.Elasticsearch) | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Elasticsearch) |
| EventStoreDB    | `eventstore/eventstore:22.10.1-buster-slim`                         | [NuGet](https://www.nuget.org/packages/Testcontainers.EventStoreDb)  | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.EventStoreDb)  |
| FakeGcsServer   | `fsouza/fake-gcs-server:1.47`                                       | [NuGet](https://www.nuget.org/packages/Testcontainers.FakeGcsServer) | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.FakeGcsServer) |
| Firestore       | `gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators` | [NuGet](https://www.nuget.org/packages/Testcontainers.Firestore)     | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Firestore)     |
| InfluxDB        | `influxdb:2.7`                                                      | [NuGet](https://www.nuget.org/packages/Testcontainers.InfluxDb)      | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.InfluxDb)      |
| K3s             | `rancher/k3s:v1.26.2-k3s1`                                          | [NuGet](https://www.nuget.org/packages/Testcontainers.K3s)           | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.K3s)           |
| Kafka           | `confluentinc/cp-kafka:6.1.9`                                       | [NuGet](https://www.nuget.org/packages/Testcontainers.Kafka)         | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Kafka)         |
| Keycloak        | `quay.io/keycloak/keycloak:21.1`                                    | [NuGet](https://www.nuget.org/packages/Testcontainers.Keycloak)      | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Keycloak)      |
| Kusto emulator  | `mcr.microsoft.com/azuredataexplorer/kustainer-linux:latest`        | [NuGet](https://www.nuget.org/packages/Testcontainers.Kusto)         | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Kusto)         |
| LocalStack      | `localstack/localstack:2.0`                                         | [NuGet](https://www.nuget.org/packages/Testcontainers.LocalStack)    | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.LocalStack)    |
| MariaDB         | `mariadb:10.10`                                                     | [NuGet](https://www.nuget.org/packages/Testcontainers.MariaDb)       | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.MariaDb)       |
| MinIO           | `minio/minio:RELEASE.2023-01-31T02-24-19Z`                          | [NuGet](https://www.nuget.org/packages/Testcontainers.Minio)         | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Minio)         |
| MongoDB         | `mongo:6.0`                                                         | [NuGet](https://www.nuget.org/packages/Testcontainers.MongoDb)       | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.MongoDb)       |
| MySQL           | `mysql:8.0`                                                         | [NuGet](https://www.nuget.org/packages/Testcontainers.MySql)         | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.MySql)         |
| NATS            | `nats:2.9`                                                          | [NuGet](https://www.nuget.org/packages/Testcontainers.Nats)          | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Nats)          |
| Neo4j           | `neo4j:5.4`                                                         | [NuGet](https://www.nuget.org/packages/Testcontainers.Neo4j)         | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Neo4j)         |
| Oracle          | `gvenzl/oracle-xe:21.3.0-slim-faststart`                            | [NuGet](https://www.nuget.org/packages/Testcontainers.Oracle)        | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Oracle)        |
| PostgreSQL      | `postgres:15.1`                                                     | [NuGet](https://www.nuget.org/packages/Testcontainers.PostgreSql)    | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.PostgreSql)    |
| PubSub          | `gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators` | [NuGet](https://www.nuget.org/packages/Testcontainers.PubSub)        | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.PubSub)        |
| RabbitMQ        | `rabbitmq:3.11`                                                     | [NuGet](https://www.nuget.org/packages/Testcontainers.RabbitMq)      | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.RabbitMq)      |
| RavenDB         | `ravendb/ravendb:5.4-ubuntu-latest`                                 | [NuGet](https://www.nuget.org/packages/Testcontainers.RavenDb)       | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.RavenDb)       |
| Redis           | `redis:7.0`                                                         | [NuGet](https://www.nuget.org/packages/Testcontainers.Redis)         | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Redis)         |
| Redpanda        | `docker.redpanda.com/redpandadata/redpanda:v22.2.1`                 | [NuGet](https://www.nuget.org/packages/Testcontainers.Redpanda)      | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.Redpanda)      |
| SQL Server      | `mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04`             | [NuGet](https://www.nuget.org/packages/Testcontainers.MsSql)         | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.MsSql)         |
| WebDriver       | `selenium/standalone-chrome:110.0`                                  | [NuGet](https://www.nuget.org/packages/Testcontainers.WebDriver)     | [Source](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Testcontainers.WebDriver)     |

## Implement a module

The Testcontainers for .NET repository contains a .NET [template](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src/Templates) to scaffold advanced modules quickly. To create and add a new module to the Testcontainers solution file, checkout the repository and install the .NET template first:

```console
git clone --branch develop git@github.com:testcontainers/testcontainers-dotnet.git
cd ./testcontainers-dotnet/
dotnet new --install ./src/Templates
```

The following CLI commands create and add a new PostgreSQL module to the solution file:

```console
dotnet new tcm --name PostgreSql --official-module true --output ./src
dotnet sln add ./src/Testcontainers.PostgreSql/Testcontainers.PostgreSql.csproj
```

!!! note

    If you decided to publish a module to [NuGet.org](https://www.nuget.org/profiles/Testcontainers) on your own, you need to set the `--official-module` flag to `false`.

A module typically consists of three classes representing the builder, the configuration and the container. The PostgreSQL module we just created above consists of the `PostgreSqlBuilder`, `PostgreSqlConfiguration` and `PostgreSqlContainer` classes.

1. The builder class sets the module default configuration and validates it. It extends the Testcontainers builder and adds or overrides members specifically to configure the module. The builder is responsible for creating a valid configuration and container instance.
2. The configuration class stores optional members to configure the module and interact with the container. Usually, these are properties like a `Username` or `Password` that are required sometime later.
3. Developers interact with the builder the most. It manages the lifecycle and provides module specific members to interact with the container. The result of the builder is an instance of the container class.

### Configure a module

The configuration classes in Testcontainers are designed to be immutable, meaning that once an instance of a configuration class has been created, its values cannot be changed. This has a number of benefits, it is more reliable, easier to understand and to share between different use cases like A/B testing.

To set the PostgreSQL module default configuration, override the read-only `DockerResourceConfiguration` property in `PostgreSqlBuilder` and set its value in both constructors. The default constructor sets `DockerResourceConfiguration` to the return value of `Init().DockerResourceConfiguration`, where the overloaded private constructor just sets the argument value. It receives an updated instance of the immutable Docker resource configuration as soon as a property changes.

The .NET template already includes this configuration, making it easy for developers to quickly get started by simply commenting out the necessary parts:

```csharp
public PostgreSqlBuilder()
  : this(new PostgreSqlConfiguration())
{
  DockerResourceConfiguration = Init().DockerResourceConfiguration;
}

private PostgreSqlBuilder(PostgreSqlConfiguration resourceConfiguration)
  : base(resourceConfiguration)
{
  DockerResourceConfiguration = resourceConfiguration;
}

protected override PostgreSqlConfiguration DockerResourceConfiguration { get; }
```

To append the PostgreSQL configurations to the default Testcontainers configurations override or comment out the `Init()` member and add the necessary configurations such as the Docker image and a wait strategy to the base implementation:

```csharp
protected override PostgreSqlBuilder Init()
{
  var waitStrategy = Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready");
  return base.Init().WithImage("postgres:15.1").WithPortBinding(5432, true).WithWaitStrategy(waitStrategy);
}
```

### Extend a module

When using the PostgreSQL Docker image, it is required to have a password set in order to run it. To demonstrate how to add a new builder member, we will use this requirement as an example.

First add a new property `Password` to the `PostgreSqlConfiguration` class. Add a `password` argument with a default value of `null` to the default constructor. This allows the builder to set individual arguments or configurations.

The overloaded `PostgreSqlConfiguration(PostgreSqlConfiguration, PostgreSqlConfiguration)` constructor takes care of merging the configurations together. The builder will receive and hold an updated instances that contains all information:

```csharp
public PostgreSqlConfiguration(string password = null)
{
  Password = password;
}

public PostgreSqlConfiguration(PostgreSqlConfiguration oldValue, PostgreSqlConfiguration newValue)
  : base(oldValue, newValue)
{
  Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
}

public string Password { get; }
```

Since the `PostgreSqlConfiguration` class is now able to store the password value, we can add a member `WithPassword(string)` to `PostgreSqlBuilder`. We not just store the password in the `PostgreSqlConfiguration` instance to construct the database connection string later, but we also set the necessary environment variable `POSTGRES_PASSWORD` to run the container:

```csharp
public PostgreSqlBuilder WithPassword(string password)
{
  return Merge(DockerResourceConfiguration, new PostgreSqlConfiguration(password: password)).WithEnvironment("POSTGRES_PASSWORD", password);
}
```

By following this approach, the `PostgreSqlContainer` class is able to access the configured values, allowing it to provide additional functionalities, such as constructing the database connection string. This enables the class to provide a more streamlined and convenient experience for developers who are working with modules:

```csharp
public string GetConnectionString()
{
  var properties = new Dictionary<string, string>();
  properties.Add("Host", Hostname);
  properties.Add("Port", GetMappedPublicPort(5432).ToString());
  properties.Add("Database", "postgres");
  properties.Add("Username", "postgres");
  properties.Add("Password", _configuration.Password);
  return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
}
```

Finally, there are two approaches to ensure that the required password is provided. Either override the `Validate()` member and check the immutable configuration instance:

```csharp
protected override void Validate()
{
  base.Validate();

  _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(PostgreSqlConfiguration.Password))
    .NotNull()
    .NotEmpty();
}
```

or extend the `Init()` member as we have already done and add `WithPassword(Guid.NewGuid().ToString())` to set a default value.

It is always a good idea to add both approaches. This way, the user can be sure that the module is properly configured, whether by themself or by default. This helps maintain a consistent and reliable experience for the user.

The repository provides reference implementations of [modules](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src). This modules are comprehensive examples and can serve as guides for you to get a better understanding of how to implement an entire module including the tests.
