# Modules

Modules are a great example of the capabilities of Testcontainers for .NET. Choose one of the pre-configurations below or find further examples in [TestcontainersContainerTest][testcontainers-container-tests] as well as in the [database][testcontainers-database-tests] or [message broker][testcontainers-message-broker-tests] tests to set up your test environment.

!!!warning

    We are redesigning modules and removing the extension method in the future. Modules will become independent projects that allow more complex and advanced features. Due to a design flaw in the current module system, we cannot distinguish between a generic and module builder. If you rely on a module you will get an obsolete warning until the next version of Testcontainers gets released. You will find more information [here](https://github.com/testcontainers/testcontainers-dotnet/issues/750#issuecomment-1412257694).

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

To append the PostgreSQL configurations to the default Testcontainers configurations override or comment out the member `Init()` and add the necessary configurations such as the Docker image and a wait strategy to the base implementation:

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

It is always a good idea to add both approaches. This way, the user can be sure that the module is properly configured, whether by themself or by default. This helps maintain a consistent and reliable experience for the user. Following it, when creating your own modules, either in-house or public you can be a role model for other developers too.

The repository provides reference implementations of [modules](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src). This modules are comprehensive examples and can serve as guides for you to get a better understanding of how to implement an entire module including the tests.

## Extension method module

Please note that this approach will be obsolete soon. We will update the documentation soon as the new modules (NuGets) are available. You find the refactored modules and sources [here](https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src).

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
