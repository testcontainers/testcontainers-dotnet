namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// A connection string provider factory.
  /// </summary>
  [PublicAPI]
  public interface IConnectionStringProviderFactory<in TContainerEntity, in TConfigurationEntity>
    where TContainerEntity : IContainer
    where TConfigurationEntity : IContainerConfiguration
  {
    /// <summary>
    /// Creates a connection string provider.
    /// </summary>
    /// <remarks>
    /// The method is invoked each time a container starts. Return a new
    /// instance on each call. Otherwise, containers built from the same
    /// builder share the connection string provider.
    /// </remarks>
    /// <returns>The connection string provider.</returns>
    IConnectionStringProvider<TContainerEntity, TConfigurationEntity> Create();
  }
}
