namespace DotNet.Testcontainers.Providers.Examples
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Providers;
  using JetBrains.Annotations;

  /// <summary>
  /// Examples demonstrating the usage of connection string providers with Testcontainers.
  /// </summary>
  [PublicAPI]
  public static class ConnectionStringProviderExamples
  {
    /// <summary>
    /// Example showing how to use the HTTP connection string provider with multiple named endpoints.
    /// </summary>
    public static async Task HttpConnectionStringProviderExample()
    {
      // Create a container with multiple HTTP endpoints
      var httpProvider = new HttpConnectionStringProvider(80, "http", "/") // Default endpoint
        .AddEndpoint("api", 80, "http", "/api/v1")
        .AddEndpoint("admin", 8080, "http", "/admin")
        .AddEndpoint("metrics", 9090, "http", "/metrics");

      var container = new ContainerBuilder()
        .WithImage("nginx:latest")
        .WithPortBinding(80)
        .WithPortBinding(8080)
        .WithPortBinding(9090)
        .WithConnectionStringProvider(httpProvider)
        .Build();

      try
      {
        await container.StartAsync();

        // Get default connection strings
        var hostUrl = container.GetConnectionString(); // Default: Host mode, default endpoint
        var containerUrl = container.GetConnectionString(ConnectionMode.Container);

        Console.WriteLine($"Default Host URL: {hostUrl}");
        Console.WriteLine($"Default Container URL: {containerUrl}");

        // Get named connection strings
        var apiHostUrl = container.GetConnectionString(ConnectionMode.Host, "api");
        var adminHostUrl = container.GetConnectionString(ConnectionMode.Host, "admin");
        var metricsContainerUrl = container.GetConnectionString(ConnectionMode.Container, "metrics");

        Console.WriteLine($"API Host URL: {apiHostUrl}");
        Console.WriteLine($"Admin Host URL: {adminHostUrl}");
        Console.WriteLine($"Metrics Container URL: {metricsContainerUrl}");

        // Using ConnectionStringIdentifier
        var apiIdentifier = ConnectionStringIdentifier.Host("api");
        var apiUrl = container.GetConnectionString(apiIdentifier);
        Console.WriteLine($"API URL (using identifier): {apiUrl}");

        // List all available connection strings
        var provider = container.GetConnectionStringProvider();
        var availableConnections = provider.GetAvailableConnectionStrings();
        Console.WriteLine($"Available connections: {string.Join(", ", availableConnections)}");

        // Example outputs:
        // Default Host URL: http://localhost:49152/
        // Default Container URL: http://172.17.0.2:80/
        // API Host URL: http://localhost:49152/api/v1
        // Admin Host URL: http://localhost:49153/admin
        // Metrics Container URL: http://172.17.0.2:9090/metrics
        // API URL (using identifier): http://localhost:49152/api/v1
        // Available connections: Host:default, Container:default, Host:api, Container:api, Host:admin, Container:admin, Host:metrics, Container:metrics
      }
      finally
      {
        await container.StopAsync();
      }
    }

    /// <summary>
    /// Example showing how to use the TCP connection string provider with multiple named connections.
    /// </summary>
    public static async Task TcpConnectionStringProviderExample()
    {
      // Create a container with multiple database connection strings
      var tcpProvider = new TcpConnectionStringProvider(5432, 
          "Host={HOST};Port={PORT};Database=testdb;Username=testuser;Password=testpass") // Default connection
        .AddConnection("admin", 5432, "Host={HOST};Port={PORT};Database=postgres;Username=postgres;Password=admin")
        .AddConnection("readonly", 5432, "Host={HOST};Port={PORT};Database=testdb;Username=reader;Password=readonly");

      var container = new ContainerBuilder()
        .WithImage("postgres:13")
        .WithEnvironment("POSTGRES_DB", "testdb")
        .WithEnvironment("POSTGRES_USER", "testuser")
        .WithEnvironment("POSTGRES_PASSWORD", "testpass")
        .WithPortBinding(5432)
        .WithConnectionStringProvider(tcpProvider)
        .Build();

      try
      {
        await container.StartAsync();

        // Get default connection strings
        var defaultHostConnection = container.GetConnectionString();
        var defaultContainerConnection = container.GetConnectionString(ConnectionMode.Container);

        Console.WriteLine($"Default Host connection: {defaultHostConnection}");
        Console.WriteLine($"Default Container connection: {defaultContainerConnection}");

        // Get named connection strings
        var adminHostConnection = container.GetConnectionString(ConnectionMode.Host, "admin");
        var readonlyContainerConnection = container.GetConnectionString(ConnectionMode.Container, "readonly");

        Console.WriteLine($"Admin Host connection: {adminHostConnection}");
        Console.WriteLine($"Readonly Container connection: {readonlyContainerConnection}");

        // Check if specific connection exists
        var provider = container.GetConnectionStringProvider();
        var hasAdminConnection = provider.HasConnectionString(ConnectionMode.Host, "admin");
        var hasMissingConnection = provider.HasConnectionString(ConnectionMode.Host, "missing");

        Console.WriteLine($"Has admin connection: {hasAdminConnection}");
        Console.WriteLine($"Has missing connection: {hasMissingConnection}");

        // Example outputs:
        // Default Host connection: Host=localhost;Port=49153;Database=testdb;Username=testuser;Password=testpass
        // Default Container connection: Host=172.17.0.2;Port=5432;Database=testdb;Username=testuser;Password=testpass
        // Admin Host connection: Host=localhost;Port=49153;Database=postgres;Username=postgres;Password=admin
        // Readonly Container connection: Host=172.17.0.2;Port=5432;Database=testdb;Username=reader;Password=readonly
        // Has admin connection: True
        // Has missing connection: False
      }
      finally
      {
        await container.StopAsync();
      }
    }

    /// <summary>
    /// Example showing how to create a custom connection string provider.
    /// </summary>
    public static async Task CustomConnectionStringProviderExample()
    {
      // Create a container with a custom connection string provider
      var container = new ContainerBuilder()
        .WithImage("redis:latest")
        .WithPortBinding(6379)
        .WithConnectionStringProvider(new RedisConnectionStringProvider())
        .Build();

      try
      {
        await container.StartAsync();

        // Get connection strings
        var hostConnectionString = container.GetConnectionString();
        var containerConnectionString = container.GetConnectionString(ConnectionMode.Container);

        Console.WriteLine($"Redis host connection string: {hostConnectionString}");
        Console.WriteLine($"Redis container connection string: {containerConnectionString}");

        // Example outputs:
        // Redis host connection string: localhost:49154
        // Redis container connection string: 172.17.0.2:6379
      }
      finally
      {
        await container.StopAsync();
      }
    }

    /// <summary>
    /// Example showing how to pass around connection string providers as abstractions.
    /// </summary>
    public static async Task ConnectionStringProviderAsAbstractionExample()
    {
      var httpContainer = new ContainerBuilder()
        .WithImage("nginx:latest")
        .WithPortBinding(8080, 80)
        .WithConnectionStringProvider(new HttpConnectionStringProvider(80))
        .Build();

      var dbContainer = new ContainerBuilder()
        .WithImage("postgres:13")
        .WithEnvironment("POSTGRES_DB", "testdb")
        .WithEnvironment("POSTGRES_USER", "testuser")
        .WithEnvironment("POSTGRES_PASSWORD", "testpass")
        .WithPortBinding(5432)
        .WithConnectionStringProvider(new TcpConnectionStringProvider(5432, 
          "Host={HOST};Port={PORT};Database=testdb;Username=testuser;Password=testpass"))
        .Build();

      try
      {
        await Task.WhenAll(httpContainer.StartAsync(), dbContainer.StartAsync());

        // Use the providers as abstractions
        ProcessService(httpContainer.GetConnectionStringProvider(), "HTTP Service");
        ProcessService(dbContainer.GetConnectionStringProvider(), "Database Service");
      }
      finally
      {
        await Task.WhenAll(httpContainer.StopAsync(), dbContainer.StopAsync());
      }
    }

    /// <summary>
    /// Example showing advanced usage with multiple named connection strings and different access patterns.
    /// </summary>
    public static async Task AdvancedNamedConnectionStringsExample()
    {
      // Create a complex service with multiple connection types
      var multiServiceProvider = new MultiServiceConnectionStringProvider();

      var container = new ContainerBuilder()
        .WithImage("multi-service:latest") // Hypothetical container with multiple services
        .WithPortBinding(80)   // Web interface
        .WithPortBinding(5432) // Database
        .WithPortBinding(6379) // Redis
        .WithPortBinding(9200) // Elasticsearch
        .WithConnectionStringProvider(multiServiceProvider)
        .Build();

      try
      {
        await container.StartAsync();

        var provider = container.GetConnectionStringProvider();

        // List all available connections
        Console.WriteLine("Available connection strings:");
        foreach (var identifier in provider.GetAvailableConnectionStrings())
        {
          Console.WriteLine($"  {identifier}: {provider.GetConnectionString(identifier)}");
        }

        // Access specific connections for different use cases
        var webHostUrl = container.GetConnectionString(ConnectionMode.Host, "web");
        var dbContainerConnection = container.GetConnectionString(ConnectionMode.Container, "database");
        var redisHostConnection = container.GetConnectionString(ConnectionMode.Host, "redis");

        Console.WriteLine($"\nWeb interface (from host): {webHostUrl}");
        Console.WriteLine($"Database (from container): {dbContainerConnection}");
        Console.WriteLine($"Redis (from host): {redisHostConnection}");

        // Dynamic connection string access
        var connectionNames = new[] { "web", "database", "redis", "elasticsearch" };
        foreach (var name in connectionNames)
        {
          if (provider.HasConnectionString(ConnectionMode.Host, name))
          {
            var connection = container.GetConnectionString(ConnectionMode.Host, name);
            Console.WriteLine($"{name} service available at: {connection}");
          }
        }
      }
      finally
      {
        await container.StopAsync();
      }
    }

    private static void ProcessService(IConnectionStringProvider provider, string serviceName)
    {
      if (provider != null)
      {
        Console.WriteLine($"{serviceName} - Default Host: {provider.GetConnectionString()}");
        
        // Show all available connections for this service
        var availableConnections = provider.GetAvailableConnectionStrings();
        foreach (var connection in availableConnections.Take(3)) // Show first 3 for brevity
        {
          Console.WriteLine($"  {connection}: {provider.GetConnectionString(connection)}");
        }
      }
    }
  }

  /// <summary>
  /// Custom Redis connection string provider implementation.
  /// </summary>
  public class RedisConnectionStringProvider : ConnectionStringProvider<IContainer, IContainerConfiguration>
  {
    /// <inheritdoc />
    protected override void BuildConnectionStrings()
    {
      if (Container == null)
      {
        throw new InvalidOperationException("Container is not available. Ensure the provider has been built.");
      }

      // Redis connection strings are typically just "host:port"
      var hostPort = Container.GetMappedPublicPort(6379);
      var hostConnectionString = $"{Container.Hostname}:{hostPort}";
      SetConnectionString(ConnectionMode.Host, hostConnectionString);

      var containerConnectionString = $"{Container.IpAddress}:6379";
      SetConnectionString(ConnectionMode.Container, containerConnectionString);
    }
  }

  /// <summary>
  /// Example of a multi-service connection string provider that provides named connections for different services.
  /// </summary>
  public class MultiServiceConnectionStringProvider : ConnectionStringProvider<IContainer, IContainerConfiguration>
  {
    /// <inheritdoc />
    protected override void BuildConnectionStrings()
    {
      if (Container == null)
      {
        throw new InvalidOperationException("Container is not available. Ensure the provider has been built.");
      }

      // Web service (HTTP)
      var webHostPort = Container.GetMappedPublicPort(80);
      SetConnectionString(ConnectionMode.Host, "web", $"http://{Container.Hostname}:{webHostPort}");
      SetConnectionString(ConnectionMode.Container, "web", $"http://{Container.IpAddress}:80");

      // Database service (PostgreSQL)
      var dbHostPort = Container.GetMappedPublicPort(5432);
      SetConnectionString(ConnectionMode.Host, "database", $"Host={Container.Hostname};Port={dbHostPort};Database=app;Username=app;Password=secret");
      SetConnectionString(ConnectionMode.Container, "database", $"Host={Container.IpAddress};Port=5432;Database=app;Username=app;Password=secret");

      // Redis service
      var redisHostPort = Container.GetMappedPublicPort(6379);
      SetConnectionString(ConnectionMode.Host, "redis", $"{Container.Hostname}:{redisHostPort}");
      SetConnectionString(ConnectionMode.Container, "redis", $"{Container.IpAddress}:6379");

      // Elasticsearch service
      var esHostPort = Container.GetMappedPublicPort(9200);
      SetConnectionString(ConnectionMode.Host, "elasticsearch", $"http://{Container.Hostname}:{esHostPort}");
      SetConnectionString(ConnectionMode.Container, "elasticsearch", $"http://{Container.IpAddress}:9200");

      // Additional connection variants
      SetConnectionString(ConnectionMode.Host, "database-admin", $"Host={Container.Hostname};Port={dbHostPort};Database=postgres;Username=postgres;Password=admin");
      SetConnectionString(ConnectionMode.Container, "database-admin", $"Host={Container.IpAddress};Port=5432;Database=postgres;Username=postgres;Password=admin");
    }
  }
}