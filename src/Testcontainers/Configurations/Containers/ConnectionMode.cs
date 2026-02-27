namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// Represents the connection mode.
  /// </summary>
  [PublicAPI]
  public enum ConnectionMode
  {
    /// <summary>
    /// The connection string for the container.
    /// </summary>
    Container,

    /// <summary>
    /// The connection string for the host.
    /// </summary>
    Host,
  }
}
