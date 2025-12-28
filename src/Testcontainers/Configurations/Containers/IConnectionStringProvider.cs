namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// A connection string provider.
  /// </summary>
  [PublicAPI]
  public interface IConnectionStringProvider
  {
    /// <summary>
    /// Gets the connection string.
    /// </summary>
    /// <param name="connectionMode">The connection mode.</param>
    /// <returns>The connection string.</returns>
    /// <exception cref="ConnectionStringProviderNotConfiguredException">Thrown when the connection string provider is not configured.</exception>
    [NotNull]
    string GetConnectionString(ConnectionMode connectionMode = ConnectionMode.Host);

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    /// <param name="name">The connection string name.</param>
    /// <param name="connectionMode">The connection mode.</param>
    /// <returns>The connection string.</returns>
    /// <exception cref="ConnectionStringProviderNotConfiguredException">Thrown when the connection string provider is not configured.</exception>
    [NotNull]
    string GetConnectionString(string name, ConnectionMode connectionMode = ConnectionMode.Host);
  }
}
