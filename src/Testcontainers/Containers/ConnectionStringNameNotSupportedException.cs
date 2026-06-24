namespace DotNet.Testcontainers.Containers
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an exception that is thrown when a named connection string is not supported.
  /// </summary>
  [PublicAPI]
  public sealed class ConnectionStringNameNotSupportedException : NotSupportedException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringNameNotSupportedException" /> class.
    /// </summary>
    /// <param name="name">The connection string name.</param>
    /// <param name="providerType">The provider type.</param>
    public ConnectionStringNameNotSupportedException(string name, Type providerType)
      : base($"The connection string name '{name}' is not supported by connection string provider '{providerType.FullName}'.")
    {
    }
  }
}
