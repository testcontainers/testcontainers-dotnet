# Connection String Provider

The Connection String Provider API provides a standardized way to access and manage connection information for Testcontainers (modules). It allows developers to customize module-provided connection strings or add their own, and to access module-specific connection strings or endpoints (e.g., database connection strings, HTTP API base addresses) in a uniform way.

!!!note

    Testcontainers modules do not yet implement this feature. Developers can use the provider to define and manage their own connection strings or endpoints. Providers will be integrated by modules in future releases.

## Example

Register a custom connection string provider via the container builder:

```csharp
IContainer container = new ContainerBuilder()
  .WithConnectionStringProvider(new MyProvider1())
  .Build();

// Implicit host connection string (default)
var hostConnectionStringImplicit = container.GetConnectionString();

// Explicit host connection string
var hostConnectionStringExplicit = container.GetConnectionString(ConnectionMode.Host);

// Container-to-container connection string
var containerConnectionString = container.GetConnectionString(ConnectionMode.Container);
```

## Implementing a custom provider

To create a custom provider, implement the generic interface: `IConnectionStringProvider<TContainer, TConfiguration>`. The `Configure(TContainer, TConfiguration)` method is invoked after the container has successfully started, ensuring that all runtime-assigned values are available.

=== "Generic provider"
    ```csharp
    public sealed class MyProvider1 : IConnectionStringProvider<IContainer, IContainerConfiguration>
    {
      public void Configure(IContainer container, IContainerConfiguration configuration)
      {
        // Initialize provider with container information.
      }

      public string GetConnectionString(ConnectionMode connectionMode = ConnectionMode.Host)
      {
        // This method returns a default connection string. The connection mode argument
        // lets you choose between a host connection or a container-to-container connection.
        return "...";
      }

      public string GetConnectionString(string name, ConnectionMode connectionMode = ConnectionMode.Host)
      {
        // This method returns a connection string for the given name. Useful for modules
        // with multiple endpoints (e.g., Azurite blob, queue, or table).
        return "...";
      }
    }
    ```

=== "Module provider"
    ```csharp
    public sealed class MyProvider2 : IConnectionStringProvider<PostgreSqlContainer, PostgreSqlConfiguration>
    {
      public void Configure(PostgreSqlContainer container, PostgreSqlConfiguration configuration)
      {
        // Initialize provider with PostgreSQL-specific container information.
      }

      public string GetConnectionString(ConnectionMode connectionMode = ConnectionMode.Host)
      {
        return "Host=localhost;Port=5432;...";
      }

      public string GetConnectionString(string name, ConnectionMode connectionMode = ConnectionMode.Host)
      {
        return "Host=localhost;Port=5432;...;SSL Mode=Require";
      }
    }
    ```
