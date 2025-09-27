namespace DotNet.Testcontainers.Providers
{
  using System;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// A generic HTTP connection string provider that provides base address URLs for HTTP-based services.
  /// This implementation can create multiple named connection strings for different endpoints.
  /// </summary>
  [PublicAPI]
  public class HttpConnectionStringProvider : ConnectionStringProvider<IContainer, IContainerConfiguration>
  {
    private readonly Dictionary<string, HttpEndpointConfig> _endpoints = new Dictionary<string, HttpEndpointConfig>();

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpConnectionStringProvider" /> class with a single default endpoint.
    /// </summary>
    /// <param name="port">The port number the service is listening on inside the container.</param>
    /// <param name="scheme">The scheme (http or https). Defaults to "http".</param>
    /// <param name="path">The optional path component. Defaults to empty string.</param>
    public HttpConnectionStringProvider(int port, string scheme = "http", string path = "")
    {
      AddEndpoint(ConnectionStringIdentifier.DefaultName, port, scheme, path);
    }

    /// <summary>
    /// Adds a named HTTP endpoint configuration.
    /// </summary>
    /// <param name="name">The endpoint name.</param>
    /// <param name="port">The port number.</param>
    /// <param name="scheme">The scheme (http or https). Defaults to "http".</param>
    /// <param name="path">The optional path component. Defaults to empty string.</param>
    /// <returns>This instance for method chaining.</returns>
    public HttpConnectionStringProvider AddEndpoint(string name, int port, string scheme = "http", string path = "")
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("Endpoint name cannot be null or empty.", nameof(name));
      }

      _endpoints[name] = new HttpEndpointConfig(port, scheme ?? "http", path ?? "");
      return this;
    }

    /// <inheritdoc />
    protected override void BuildConnectionStrings()
    {
      if (Container == null)
      {
        throw new InvalidOperationException("Container is not available. Ensure the provider has been built.");
      }

      foreach (var endpoint in _endpoints)
      {
        var name = endpoint.Key;
        var config = endpoint.Value;

        // Host connection string uses the container's hostname and mapped port
        var hostPort = Container.GetMappedPublicPort(config.Port);
        var hostConnectionString = $"{config.Scheme}://{Container.Hostname}:{hostPort}{config.Path}";
        SetConnectionString(ConnectionMode.Host, name, hostConnectionString);

        // Container connection string uses the container's IP address and internal port
        var containerConnectionString = $"{config.Scheme}://{Container.IpAddress}:{config.Port}{config.Path}";
        SetConnectionString(ConnectionMode.Container, name, containerConnectionString);
      }
    }

    private readonly struct HttpEndpointConfig
    {
      public HttpEndpointConfig(int port, string scheme, string path)
      {
        Port = port;
        Scheme = scheme;
        Path = path;
      }

      public int Port { get; }
      public string Scheme { get; }
      public string Path { get; }
    }
  }

  /// <summary>
  /// A generic TCP connection string provider that provides connection strings for TCP-based services.
  /// This implementation can create multiple named connection strings for different connection types.
  /// </summary>
  [PublicAPI]
  public class TcpConnectionStringProvider : ConnectionStringProvider<IContainer, IContainerConfiguration>
  {
    private readonly Dictionary<string, TcpConnectionConfig> _connections = new Dictionary<string, TcpConnectionConfig>();

    /// <summary>
    /// Initializes a new instance of the <see cref="TcpConnectionStringProvider" /> class with a single default connection.
    /// </summary>
    /// <param name="port">The port number the service is listening on inside the container.</param>
    /// <param name="template">The connection string template. Use {HOST} and {PORT} placeholders.</param>
    /// <example>
    /// For PostgreSQL: "Host={HOST};Port={PORT};Database=testdb;Username=user;Password=pass"
    /// For Redis: "{HOST}:{PORT}"
    /// </example>
    public TcpConnectionStringProvider(int port, string template)
    {
      AddConnection(ConnectionStringIdentifier.DefaultName, port, template);
    }

    /// <summary>
    /// Adds a named TCP connection configuration.
    /// </summary>
    /// <param name="name">The connection name.</param>
    /// <param name="port">The port number.</param>
    /// <param name="template">The connection string template. Use {HOST} and {PORT} placeholders.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <example>
    /// provider.AddConnection("admin", 5432, "Host={HOST};Port={PORT};Database=postgres;Username=admin;Password=admin")
    ///         .AddConnection("readonly", 5433, "Host={HOST};Port={PORT};Database=postgres;Username=reader;Password=reader");
    /// </example>
    public TcpConnectionStringProvider AddConnection(string name, int port, string template)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("Connection name cannot be null or empty.", nameof(name));
      }

      if (string.IsNullOrEmpty(template))
      {
        throw new ArgumentException("Template cannot be null or empty.", nameof(template));
      }

      _connections[name] = new TcpConnectionConfig(port, template);
      return this;
    }

    /// <inheritdoc />
    protected override void BuildConnectionStrings()
    {
      if (Container == null)
      {
        throw new InvalidOperationException("Container is not available. Ensure the provider has been built.");
      }

      foreach (var connection in _connections)
      {
        var name = connection.Key;
        var config = connection.Value;

        // Host connection string uses the container's hostname and mapped port
        var hostPort = Container.GetMappedPublicPort(config.Port);
        var hostConnectionString = config.Template
          .Replace("{HOST}", Container.Hostname)
          .Replace("{PORT}", hostPort.ToString());
        SetConnectionString(ConnectionMode.Host, name, hostConnectionString);

        // Container connection string uses the container's IP address and internal port
        var containerConnectionString = config.Template
          .Replace("{HOST}", Container.IpAddress)
          .Replace("{PORT}", config.Port.ToString());
        SetConnectionString(ConnectionMode.Container, name, containerConnectionString);
      }
    }

    private readonly struct TcpConnectionConfig
    {
      public TcpConnectionConfig(int port, string template)
      {
        Port = port;
        Template = template;
      }

      public int Port { get; }
      public string Template { get; }
    }
  }
}