namespace DotNet.Testcontainers.Containers
{
  using System;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an exception that is thrown when a connection mode is not supported.
  /// </summary>
  [PublicAPI]
  public sealed class ConnectionStringModeNotSupportedException : NotSupportedException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringModeNotSupportedException" /> class.
    /// </summary>
    /// <param name="connectionMode">The connection mode.</param>
    /// <param name="providerType">The provider type.</param>
    public ConnectionStringModeNotSupportedException(ConnectionMode connectionMode, Type providerType)
      : base($"The connection mode '{connectionMode}' is not supported by connection string provider '{providerType.FullName}'.")
    {
    }
  }
}
