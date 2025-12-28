namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IConnectionStringProvider" />
  [PublicAPI]
  public interface IConnectionStringProvider<in TContainerEntity, in TConfigurationEntity> : IConnectionStringProvider
    where TContainerEntity : IContainer
    where TConfigurationEntity : IContainerConfiguration
  {
    /// <summary>
    /// Configures the connection string provider.
    /// </summary>
    /// <param name="container">The container instance.</param>
    /// <param name="configuration">The container configuration.</param>
    void Configure(TContainerEntity container, TConfigurationEntity configuration);
  }
}
