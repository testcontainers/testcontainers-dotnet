namespace DotNet.Testcontainers.Networks.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// Docker network driver.
  /// </summary>
  public readonly struct NetworkDriver
  {
    /// <summary>
    /// Gets network driver bridge.
    /// </summary>
    public static readonly NetworkDriver Bridge = new NetworkDriver("bridge");

    /// <summary>
    /// Gets network driver host.
    /// </summary>
    public static readonly NetworkDriver Host = new NetworkDriver("host");

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkDriver" /> class.
    /// </summary>
    /// <param name="value">The network driver.</param>
    private NetworkDriver(string value)
    {
      this.Value = value;
    }

    /// <summary>
    /// Gets the string representation of the Docker network driver.
    /// </summary>
    [NotNull]
    public string Value { get; }
  }
}
