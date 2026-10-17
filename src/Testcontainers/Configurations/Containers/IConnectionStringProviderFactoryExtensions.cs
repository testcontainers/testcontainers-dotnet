namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Containers;

  /// <summary>
  /// Provides extension methods for the <see cref="IConnectionStringProviderFactory{TContainerEntity, TConfigurationEntity}" /> interface.
  /// </summary>
  internal static class IConnectionStringProviderFactoryExtensions
  {
    /// <summary>
    /// Creates and configures a connection string provider.
    /// </summary>
    /// <typeparam name="TContainerEntity">The container entity.</typeparam>
    /// <typeparam name="TConfigurationEntity">The configuration entity.</typeparam>
    /// <param name="connectionStringProviderFactory">The connection string provider factory.</param>
    /// <param name="container">The container instance.</param>
    /// <param name="configuration">The container configuration.</param>
    /// <returns>The configured connection string provider.</returns>
    public static IConnectionStringProvider Create<TContainerEntity, TConfigurationEntity>(this IConnectionStringProviderFactory<TContainerEntity, TConfigurationEntity> connectionStringProviderFactory, IContainer container, IContainerConfiguration configuration)
      where TContainerEntity : IContainer
      where TConfigurationEntity : IContainerConfiguration
    {
      var connectionStringProvider = connectionStringProviderFactory.Create();
      connectionStringProvider.Configure((TContainerEntity)container, (TConfigurationEntity)configuration);
      return connectionStringProvider;
    }
  }
}
