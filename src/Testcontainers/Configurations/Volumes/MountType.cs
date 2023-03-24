namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// Docker mount type.
  /// </summary>
  [PublicAPI]
  public readonly struct MountType
  {
    /// <summary>
    /// The 'bind' mount type.
    /// </summary>
    [PublicAPI]
    public static readonly MountType Bind = new MountType("bind");

    /// <summary>
    /// The 'volume' mount type.
    /// </summary>
    [PublicAPI]
    public static readonly MountType Volume = new MountType("volume");

    /// <summary>
    /// The 'tmpfs' mount type.
    /// </summary>
    [PublicAPI]
    public static readonly MountType Tmpfs = new MountType("tmpfs");

    /// <summary>
    /// The 'npipe' mount type.
    /// </summary>
    [PublicAPI]
    public static readonly MountType NamedPipe = new MountType("npipe");

    /// <summary>
    /// Initializes a new instance of the <see cref="MountType" /> struct.
    /// </summary>
    /// <param name="type">The mount type.</param>
    private MountType(string type)
    {
      Type = type;
    }

    /// <summary>
    /// Gets the mount type.
    /// </summary>
    [PublicAPI]
    [NotNull]
    public string Type { get; }
  }
}
