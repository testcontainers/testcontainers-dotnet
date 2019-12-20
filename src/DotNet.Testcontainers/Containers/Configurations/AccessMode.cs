namespace DotNet.Testcontainers.Containers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// Docker volume access modes.
  /// </summary>
  public readonly struct AccessMode
  {
    /// <summary>
    /// Gets access modes ReadOnly.
    /// </summary>
    public static readonly AccessMode ReadOnly = new AccessMode("ro");

    /// <summary>
    /// Gets access modes ReadWrite.
    /// </summary>
    public static readonly AccessMode ReadWrite = new AccessMode("rw");

    /// <summary>
    /// Gets the string representation of the Docker volume access mode.
    /// </summary>
    [NotNull]
    public string Value { get; }

    private AccessMode(string value)
    {
      this.Value = value;
    }
  }
}
