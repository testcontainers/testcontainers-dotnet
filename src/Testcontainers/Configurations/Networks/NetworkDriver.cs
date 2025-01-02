namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// Docker network driver.
  /// </summary>
  [PublicAPI]
  public readonly struct NetworkDriver
  {
    /// <summary>
    /// Gets network driver bridge.
    /// </summary>
    [PublicAPI]
    public static readonly NetworkDriver Bridge = new NetworkDriver("bridge");

    /// <summary>
    /// Gets network driver host.
    /// </summary>
    [PublicAPI]
    public static readonly NetworkDriver Host = new NetworkDriver("host");

    /// <summary>
    /// Gets network driver nat.
    /// </summary>
    [PublicAPI]
    public static readonly NetworkDriver Nat = new NetworkDriver("nat");

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkDriver" /> struct.
    /// </summary>
    /// <param name="value">The network driver.</param>
    private NetworkDriver(string value)
    {
      Value = value;
    }

    /// <summary>
    /// Gets the string representation of the Docker network driver.
    /// </summary>
    [PublicAPI]
    [NotNull]
    public string Value { get; }
  }
}
