namespace DotNet.Testcontainers.Providers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// Abstract base class for connection string providers.
  /// </summary>
  /// <typeparam name="TContainer">The container type.</typeparam>
  /// <typeparam name="TConfiguration">The configuration type.</typeparam>
  [PublicAPI]
  public abstract class ConnectionStringProvider<TContainer, TConfiguration> : IConnectionStringProvider<TContainer, TConfiguration>
    where TContainer : IContainer
    where TConfiguration : IContainerConfiguration
  {
    private readonly Dictionary<ConnectionStringIdentifier, string> _connectionStrings = new Dictionary<ConnectionStringIdentifier, string>();

    /// <summary>
    /// Gets the container instance.
    /// </summary>
    [CanBeNull]
    protected TContainer Container { get; private set; }

    /// <summary>
    /// Gets the configuration instance.
    /// </summary>
    [CanBeNull]
    protected TConfiguration Configuration { get; private set; }

    /// <inheritdoc />
    public virtual void Build(TContainer container, TConfiguration configuration)
    {
      Container = container ?? throw new ArgumentNullException(nameof(container));
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

      BuildConnectionStrings();
    }

    /// <inheritdoc />
    public virtual string GetConnectionString()
    {
      return GetConnectionString(ConnectionMode.Host, ConnectionStringIdentifier.DefaultName);
    }

    /// <inheritdoc />
    public virtual string GetConnectionString(ConnectionMode connectionMode)
    {
      return GetConnectionString(connectionMode, ConnectionStringIdentifier.DefaultName);
    }

    /// <inheritdoc />
    public virtual string GetConnectionString(ConnectionMode connectionMode, string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("Connection string name cannot be null or empty.", nameof(name));
      }

      return GetConnectionString(new ConnectionStringIdentifier(connectionMode, name));
    }

    /// <inheritdoc />
    public virtual string GetConnectionString(ConnectionStringIdentifier identifier)
    {
      if (_connectionStrings.TryGetValue(identifier, out var connectionString))
      {
        return connectionString;
      }

      throw new InvalidOperationException($"Connection string '{identifier}' is not available. Available connection strings: {string.Join(", ", _connectionStrings.Keys)}");
    }

    /// <inheritdoc />
    public virtual IReadOnlyCollection<ConnectionStringIdentifier> GetAvailableConnectionStrings()
    {
      return _connectionStrings.Keys.ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public virtual bool HasConnectionString(ConnectionStringIdentifier identifier)
    {
      return _connectionStrings.ContainsKey(identifier);
    }

    /// <inheritdoc />
    public virtual bool HasConnectionString(ConnectionMode connectionMode, string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        return false;
      }

      return HasConnectionString(new ConnectionStringIdentifier(connectionMode, name));
    }

    /// <summary>
    /// Sets the connection string for the specified connection mode with the default name.
    /// </summary>
    /// <param name="connectionMode">The connection mode.</param>
    /// <param name="connectionString">The connection string.</param>
    protected void SetConnectionString(ConnectionMode connectionMode, [NotNull] string connectionString)
    {
      SetConnectionString(connectionMode, ConnectionStringIdentifier.DefaultName, connectionString);
    }

    /// <summary>
    /// Sets the connection string for the specified connection mode and name.
    /// </summary>
    /// <param name="connectionMode">The connection mode.</param>
    /// <param name="name">The connection string name.</param>
    /// <param name="connectionString">The connection string.</param>
    protected void SetConnectionString(ConnectionMode connectionMode, [NotNull] string name, [NotNull] string connectionString)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("Connection string name cannot be null or empty.", nameof(name));
      }

      SetConnectionString(new ConnectionStringIdentifier(connectionMode, name), connectionString);
    }

    /// <summary>
    /// Sets the connection string for the specified identifier.
    /// </summary>
    /// <param name="identifier">The connection string identifier.</param>
    /// <param name="connectionString">The connection string.</param>
    protected void SetConnectionString(ConnectionStringIdentifier identifier, [NotNull] string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
      }

      _connectionStrings[identifier] = connectionString;
    }

    /// <summary>
    /// Abstract method to build the connection strings for both Host and Container modes.
    /// Derived classes must implement this method to set connection strings using SetConnectionString methods.
    /// </summary>
    protected abstract void BuildConnectionStrings();
  }
}