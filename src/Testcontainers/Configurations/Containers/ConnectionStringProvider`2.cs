namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Containers;

  /// <inheritdoc cref="IConnectionStringProvider{TContainerEntity, TConfigurationEntity}" />
  internal sealed class ConnectionStringProvider<TContainerEntity, TConfigurationEntity> : IConnectionStringProvider<IContainer, IContainerConfiguration>, IConnectionStringProviderFactory
    where TContainerEntity : IContainer
    where TConfigurationEntity : IContainerConfiguration
  {
    private readonly Func<IConnectionStringProvider<TContainerEntity, TConfigurationEntity>> _connectionStringProviderFactory;

    private IConnectionStringProvider<TContainerEntity, TConfigurationEntity> _connectionStringProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringProvider{TContainerEntity, TConfigurationEntity}" /> class.
    /// </summary>
    /// <param name="connectionStringProviderFactory">The connection string provider factory.</param>
    public ConnectionStringProvider(Func<IConnectionStringProvider<TContainerEntity, TConfigurationEntity>> connectionStringProviderFactory)
    {
      _connectionStringProviderFactory = connectionStringProviderFactory;
    }

    /// <inheritdoc />
    public IConnectionStringProvider<IContainer, IContainerConfiguration> Create()
    {
      return new ConnectionStringProvider<TContainerEntity, TConfigurationEntity>(_connectionStringProviderFactory);
    }

    /// <inheritdoc />
    public void Configure(IContainer container, IContainerConfiguration configuration)
    {
      _connectionStringProvider = _connectionStringProviderFactory();
      _connectionStringProvider.Configure((TContainerEntity)container, (TConfigurationEntity)configuration);
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
