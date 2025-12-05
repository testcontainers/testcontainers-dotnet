namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Containers;

  /// <inheritdoc cref="IConnectionStringProvider{TContainerEntity, TConfigurationEntity}" />
  internal sealed class ConnectionStringProvider<TContainerEntity, TConfigurationEntity> : IConnectionStringProvider<IContainer, IContainerConfiguration>
    where TContainerEntity : IContainer
    where TConfigurationEntity : IContainerConfiguration
  {
    private readonly IConnectionStringProvider<TContainerEntity, TConfigurationEntity> _connectionStringProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringProvider{TContainerEntity, TConfigurationEntity}" /> class.
    /// </summary>
    /// <param name="connectionStringProvider">The connection string provider.</param>
    public ConnectionStringProvider(IConnectionStringProvider<TContainerEntity, TConfigurationEntity> connectionStringProvider)
    {
      _connectionStringProvider = connectionStringProvider;
    }

    /// <inheritdoc />
    public void Configure(IContainer container, IContainerConfiguration configuration)
    {
      if (container is not TContainerEntity typedContainer)
      {
        throw new InvalidCastException($"Expected container type '{typeof(TContainerEntity).FullName}', but received '{container.GetType().FullName}'.");
      }

      if (configuration is not TConfigurationEntity typedConfiguration)
      {
        throw new InvalidCastException($"Expected configuration type '{typeof(TConfigurationEntity).FullName}', but received '{configuration.GetType().FullName}'.");
      }

      _connectionStringProvider.Configure(typedContainer, typedConfiguration);
    }

    /// <inheritdoc />
    public string GetConnectionString(ConnectionMode connectionMode = ConnectionMode.Host)
    {
      return _connectionStringProvider.GetConnectionString(connectionMode);
    }

    /// <inheritdoc />
    public string GetConnectionString(string name, ConnectionMode connectionMode = ConnectionMode.Host)
    {
      return _connectionStringProvider.GetConnectionString(name, connectionMode);
    }
  }
}
