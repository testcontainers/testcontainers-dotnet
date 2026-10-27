namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Containers;

  /// <inheritdoc cref="IConnectionStringProviderFactory{TContainerEntity, TConfigurationEntity}" />
  internal sealed class ConnectionStringProviderFactory<TContainerEntity, TConfigurationEntity> : IConnectionStringProviderFactory<TContainerEntity, TConfigurationEntity>
    where TContainerEntity : IContainer
    where TConfigurationEntity : IContainerConfiguration
  {
    private readonly Func<IConnectionStringProvider<TContainerEntity, TConfigurationEntity>> _connectionStringProviderFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringProviderFactory{TContainerEntity, TConfigurationEntity}" /> class.
    /// </summary>
    /// <param name="connectionStringProviderFactory">The connection string provider factory.</param>
    public ConnectionStringProviderFactory(Func<IConnectionStringProvider<TContainerEntity, TConfigurationEntity>> connectionStringProviderFactory)
    {
      _connectionStringProviderFactory = connectionStringProviderFactory;
    }

    /// <inheritdoc />
    public IConnectionStringProvider<TContainerEntity, TConfigurationEntity> Create()
    {
      return _connectionStringProviderFactory();
    }
  }
}
