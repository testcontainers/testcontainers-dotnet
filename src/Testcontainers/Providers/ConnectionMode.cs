namespace DotNet.Testcontainers.Providers
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents the connection mode for communication with a container.
  /// </summary>
  [PublicAPI]
  public enum ConnectionMode
  {
    /// <summary>
    /// Connection string for communication from the test host to the container.
    /// </summary>
    Host,

    /// <summary>
    /// Connection string for communication from one container to another container.
    /// </summary>
    Container
  }

  /// <summary>
  /// Represents an identifier for a named connection string with a specific mode.
  /// </summary>
  [PublicAPI]
  public readonly struct ConnectionStringIdentifier : IEquatable<ConnectionStringIdentifier>
  {
    /// <summary>
    /// The default connection string name.
    /// </summary>
    public const string DefaultName = "default";

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringIdentifier" /> struct.
    /// </summary>
    /// <param name="mode">The connection mode.</param>
    /// <param name="name">The connection string name. Defaults to "default".</param>
    public ConnectionStringIdentifier(ConnectionMode mode, string name = DefaultName)
    {
      Mode = mode;
      Name = name ?? DefaultName;
    }

    /// <summary>
    /// Gets the connection mode.
    /// </summary>
    public ConnectionMode Mode { get; }

    /// <summary>
    /// Gets the connection string name.
    /// </summary>
    [NotNull]
    public string Name { get; }

    /// <summary>
    /// Creates a host connection identifier with the specified name.
    /// </summary>
    /// <param name="name">The connection string name. Defaults to "default".</param>
    /// <returns>A host connection identifier.</returns>
    public static ConnectionStringIdentifier Host(string name = DefaultName)
    {
      return new ConnectionStringIdentifier(ConnectionMode.Host, name);
    }

    /// <summary>
    /// Creates a container connection identifier with the specified name.
    /// </summary>
    /// <param name="name">The connection string name. Defaults to "default".</param>
    /// <returns>A container connection identifier.</returns>
    public static ConnectionStringIdentifier Container(string name = DefaultName)
    {
      return new ConnectionStringIdentifier(ConnectionMode.Container, name);
    }

    /// <inheritdoc />
    public bool Equals(ConnectionStringIdentifier other)
    {
      return Mode == other.Mode && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      return obj is ConnectionStringIdentifier other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = (int)Mode;
        hashCode = (hashCode * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
        return hashCode;
      }
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"{Mode}:{Name}";
    }

    /// <summary>
    /// Determines whether two connection string identifiers are equal.
    /// </summary>
    /// <param name="left">The left identifier.</param>
    /// <param name="right">The right identifier.</param>
    /// <returns>True if the identifiers are equal; otherwise, false.</returns>
    public static bool operator ==(ConnectionStringIdentifier left, ConnectionStringIdentifier right)
    {
      return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two connection string identifiers are not equal.
    /// </summary>
    /// <param name="left">The left identifier.</param>
    /// <param name="right">The right identifier.</param>
    /// <returns>True if the identifiers are not equal; otherwise, false.</returns>
    public static bool operator !=(ConnectionStringIdentifier left, ConnectionStringIdentifier right)
    {
      return !left.Equals(right);
    }
  }
}