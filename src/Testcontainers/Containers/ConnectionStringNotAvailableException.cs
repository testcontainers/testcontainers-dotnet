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
    /// <param name="providerType">The provider type.</param>
    /// <param name="connectionMode">The connection mode.</param>
    public ConnectionStringNotAvailableException(Type providerType, ConnectionMode connectionMode)
      : base($"The connection string provider '{providerType.FullName}' did not return a connection string for connection mode '{connectionMode}'.")
    {
    }
  }
}
