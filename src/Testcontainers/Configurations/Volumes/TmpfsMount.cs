namespace DotNet.Testcontainers.Configurations
{
  /// <inheritdoc cref="IMount" />
  internal readonly struct TmpfsMount : IMount
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TmpfsMount"/> struct.
    /// </summary>
    /// <param name="target">The absolute path where the tmpfs mount is mounted in the container.</param>
    /// <param name="accessMode">The Docker volume access mode.</param>
    public TmpfsMount(string target, AccessMode accessMode)
    {
      this.Type = MountType.Tmpfs;
      this.Source = string.Empty;
      this.Target = target;
      this.AccessMode = accessMode;
    }

    /// <inheritdoc />
    public MountType Type { get; }

    /// <inheritdoc />
    public AccessMode AccessMode { get; }

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public string Target { get; }
  }
}
