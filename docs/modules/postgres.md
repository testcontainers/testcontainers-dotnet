# PostgreSQL

[PostgreSQL](https://www.postgresql.org/) is a powerful, open-source relational database management system (RDBMS) used to store, manage, and retrieve structured data.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.PostgreSql
```

You can start a PostgreSQL container instance from any .NET application. To create and start a container instance with the default configuration, use the module-specific builder as shown below:

=== "Start a PostgreSQL Container"
    ```csharp
    var postgreSqlContainer = new PostgreSqlBuilder("postgres:15.1").Build();
    await postgreSqlContainer.StartAsync();
    ```

The following example utilizes the [xUnit.net](/test_frameworks/xunit_net/) module to reduce overhead by automatically managing the lifecycle of the dependent container instance. It creates and starts the container using the module-specific builder and injects it as a shared class fixture into the test class.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.PostgreSql.Tests/PostgreSqlContainerTest.cs:UsePostgreSqlContainer"
    ```

## SSL

Use `WithSsl` to enable TLS and map the server certificates. Configure the client connection string with `SslMode` and (for validation) the CA certificate.

!!! note

    When SSL is enabled, Testcontainers doesn't set the SSL mode for the connection string. You'll need to choose the `SslMode` and configure it yourself.

!!! note

    When SSL is enabled, Testcontainers overrides the entrypoint to copy the certificates for the user that runs PostgreSQL and then runs `docker-entrypoint.sh`, not an entrypoint defined by a custom image. If you set an entrypoint with `WithEntrypoint`, Testcontainers doesn't override it. You'll need to copy the certificates from `/etc/ssl/postgresql` to `/var/run/postgresql/ssl` yourself and make sure only the PostgreSQL user can access them.

```csharp
--8<-- "tests/Testcontainers.PostgreSql.Tests/PostgreSqlContainerTest.cs:PostgreSqlSslBuilder"
```

```csharp
--8<-- "tests/Testcontainers.PostgreSql.Tests/PostgreSqlContainerTest.cs:PostgreSqlSslConnectionString"
```

### VerifyFull and SANs

`SslMode=VerifyFull` additionally validates that the host in the connection string matches a subject alternative name (SAN) of the server certificate. Testcontainers usually resolves the container host to the IP address `127.0.0.1`. The certificate must include a matching IP SAN, for example `IP:127.0.0.1`:

```csharp
--8<-- "tests/Testcontainers.PostgreSql.Tests/PostgreSqlContainerTest.cs:PostgreSqlSslVerifyFull"
```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.PostgreSql.Tests/Testcontainers.PostgreSql.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"
