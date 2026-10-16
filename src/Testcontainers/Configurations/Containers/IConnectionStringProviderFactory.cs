namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Containers;

  /// <summary>
  /// A connection string provider factory.
  /// </summary>
  internal interface IConnectionStringProviderFactory
  {
    /// <summary>
    /// Creates a new connection string provider for a container.
    /// </summary>
    /// <returns>The connection string provider.</returns>
    IConnectionStringProvider<IContainer, IContainerConfiguration> Create();
  }
}
