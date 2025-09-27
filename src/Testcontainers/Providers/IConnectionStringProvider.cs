namespace DotNet.Testcontainers.Providers
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// A connection string provider that can retrieve connection strings for container communication.
  /// </summary>
  [PublicAPI]
  public interface IConnectionStringProvider
  {
    /// <summary>
    /// Gets the default connection string for the Host connection mode.
    /// </summary>
    /// <returns>The connection string for host to container communication.</returns>
    [NotNull]
    string GetConnectionString();

    /// <summary>
    /// Gets the default connection string for the specified connection mode.
    /// </summary>
    /// <param name="connectionMode">The connection mode (Host or Container).</param>
    /// <returns>The connection string for the specified connection mode.</returns>
    [NotNull]
    string GetConnectionString(ConnectionMode connectionMode);

    /// <summary>
    /// Gets the connection string for the specified connection mode and name.
    /// </summary>
    /// <param name="connectionMode">The connection mode (Host or Container).</param>
    /// <param name="name">The connection string name.</param>
    /// <returns>The connection string for the specified connection mode and name.</returns>
    [NotNull]
    string GetConnectionString(ConnectionMode connectionMode, [NotNull] string name);

    /// <summary>
    /// Gets the connection string using a connection string identifier.
    /// </summary>
    /// <param name="identifier">The connection string identifier.</param>
    /// <returns>The connection string for the specified identifier.</returns>
    [NotNull]
    string GetConnectionString(ConnectionStringIdentifier identifier);

    /// <summary>
    /// Gets all available connection string identifiers.
    /// </summary>
    /// <returns>A collection of all available connection string identifiers.</returns>
    [NotNull]
    IReadOnlyCollection<ConnectionStringIdentifier> GetAvailableConnectionStrings();

    /// <summary>
    /// Checks if a connection string exists for the specified identifier.
    /// </summary>
    /// <param name="identifier">The connection string identifier.</param>
    /// <returns>True if the connection string exists; otherwise, false.</returns>
    bool HasConnectionString(ConnectionStringIdentifier identifier);

    /// <summary>
    /// Checks if a connection string exists for the specified connection mode and name.
    /// </summary>
    /// <param name="connectionMode">The connection mode.</param>
    /// <param name="name">The connection string name.</param>
    /// <returns>True if the connection string exists; otherwise, false.</returns>
    bool HasConnectionString(ConnectionMode connectionMode, [NotNull] string name);
  }

  /// <summary>
  /// A generic connection string provider that can be configured with specific container and configuration types.
  /// </summary>
  /// <typeparam name="TContainer">The container type.</typeparam>
  /// <typeparam name="TConfiguration">The configuration type.</typeparam>
  [PublicAPI]
  public interface IConnectionStringProvider<in TContainer, in TConfiguration> : IConnectionStringProvider
    where TContainer : IContainer
    where TConfiguration : IContainerConfiguration
  {
    /// <summary>
    /// Builds or initializes the connection string provider with the container and configuration.
    /// </summary>
    /// <param name="container">The container instance.</param>
    /// <param name="configuration">The configuration instance.</param>
    void Build([NotNull] TContainer container, [NotNull] TConfiguration configuration);
  }
}