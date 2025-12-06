namespace DotNet.Testcontainers.Containers
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an exception that is thrown when the connection string provider is not configured.
  /// </summary>
  [PublicAPI]
  public sealed class ConnectionStringProviderNotConfiguredException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringProviderNotConfiguredException" /> class.
    /// </summary>
    public ConnectionStringProviderNotConfiguredException()
      : base("No connection string provider is configured for this container.")
    {
    }
  }
}
