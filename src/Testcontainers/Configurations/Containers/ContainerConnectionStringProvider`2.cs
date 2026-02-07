namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// Provides a base implementation for container connection string providers.
  /// </summary>
  [PublicAPI]
  public abstract class ContainerConnectionStringProvider<TContainerEntity, TConfigurationEntity> : IConnectionStringProvider<TContainerEntity, TConfigurationEntity>
    where TContainerEntity : IContainer
    where TConfigurationEntity : IContainerConfiguration
  {
    /// <summary>
    /// Gets the configured container instance.
    /// </summary>
    protected TContainerEntity Container { get; private set; }

    /// <summary>
    /// Gets the configured container configuration.
    /// </summary>
    protected TConfigurationEntity Configuration { get; private set; }

    /// <inheritdoc />
    public virtual void Configure(TContainerEntity container, TConfigurationEntity configuration)
    {
      Container = container;
      Configuration = configuration;
    }

    /// <inheritdoc />
    /// <exception cref="ConnectionStringNotAvailableException">Thrown when the requested connection string is not available.</exception>
    /// <exception cref="ConnectionStringModeNotSupportedException">Thrown when the requested connection mode is not supported.</exception>
    public virtual string GetConnectionString(ConnectionMode connectionMode = ConnectionMode.Host)
    {
      switch (connectionMode)
      {
        case ConnectionMode.Host:
          return GetRequiredConnectionString(GetHostConnectionString(), ConnectionMode.Host);
        case ConnectionMode.Container:
          return GetRequiredConnectionString(GetContainerConnectionString(), ConnectionMode.Container);
        default:
          throw new ConnectionStringModeNotSupportedException(connectionMode, GetType());
      }
    }

    /// <inheritdoc />
    /// <exception cref="ConnectionStringNotAvailableException">Thrown when the requested connection string is not available.</exception>
    /// <exception cref="ConnectionStringModeNotSupportedException">Thrown when the requested connection mode is not supported.</exception>
    /// <exception cref="ConnectionStringNameNotSupportedException">Thrown when the requested connection string name is not supported.</exception>
    public virtual string GetConnectionString(string name, ConnectionMode connectionMode = ConnectionMode.Host)
    {
      return string.IsNullOrEmpty(name) ? GetConnectionString(connectionMode) : throw new ConnectionStringNameNotSupportedException(name, GetType());
    }

    /// <summary>
    /// Gets the host connection string.
    /// </summary>
    /// <returns>The host connection string.</returns>
    protected abstract string GetHostConnectionString();

    /// <summary>
    /// Gets the container connection string.
    /// </summary>
    /// <returns>The container connection string.</returns>
    protected virtual string GetContainerConnectionString()
    {
      throw new ConnectionStringModeNotSupportedException(ConnectionMode.Container, GetType());
    }

    /// <summary>
    /// Ensures the connection string is present.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="connectionMode">The connection mode.</param>
    /// <returns>The connection string.</returns>
    protected string GetRequiredConnectionString(string connectionString, ConnectionMode connectionMode)
    {
      return string.IsNullOrEmpty(connectionString) ? throw new ConnectionStringNotAvailableException(connectionMode, GetType()) : connectionString;
    }
  }
}
