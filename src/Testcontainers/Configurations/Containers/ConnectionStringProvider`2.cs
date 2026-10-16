namespace DotNet.Testcontainers.Configurations
{
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
