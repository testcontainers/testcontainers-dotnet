namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Containers;

  /// <inheritdoc cref="IConnectionStringProviderFactory{TContainerEntity, TConfigurationEntity}" />
  internal sealed class ConnectionStringProviderFactory<TContainerEntity, TConfigurationEntity> : IConnectionStringProviderFactory<IContainer, IContainerConfiguration>
    where TContainerEntity : IContainer
    where TConfigurationEntity : IContainerConfiguration
  {
    private readonly IConnectionStringProviderFactory<TContainerEntity, TConfigurationEntity> _connectionStringProviderFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringProviderFactory{TContainerEntity, TConfigurationEntity}" /> class.
    /// </summary>
    /// <param name="connectionStringProviderFactory">The connection string provider factory.</param>
    public ConnectionStringProviderFactory(IConnectionStringProviderFactory<TContainerEntity, TConfigurationEntity> connectionStringProviderFactory)
    {
      _connectionStringProviderFactory = connectionStringProviderFactory;
    }

    /// <inheritdoc />
    public IConnectionStringProvider<IContainer, IContainerConfiguration> Create()
    {
      return new ConnectionStringProvider<TContainerEntity, TConfigurationEntity>(_connectionStringProviderFactory.Create());
    }
  }
}
