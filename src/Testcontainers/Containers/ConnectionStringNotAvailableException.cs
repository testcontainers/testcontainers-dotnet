namespace DotNet.Testcontainers.Containers
{
  using System;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an exception that is thrown when a connection string cannot be resolved.
  /// </summary>
  [PublicAPI]
  public sealed class ConnectionStringNotAvailableException : InvalidOperationException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringNotAvailableException" /> class.
    /// </summary>
    /// <param name="connectionMode">The connection mode.</param>
    /// <param name="providerType">The provider type.</param>
    public ConnectionStringNotAvailableException(ConnectionMode connectionMode, Type providerType)
      : base($"The connection string provider '{providerType.FullName}' did not return a connection string for connection mode '{connectionMode}'.")
    {
    }
  }
}
