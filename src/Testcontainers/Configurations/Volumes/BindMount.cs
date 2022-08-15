namespace DotNet.Testcontainers.Configurations
{
  /// <inheritdoc cref="IMount" />
  internal readonly struct BindMount : IMount
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BindMount" /> struct.
    /// </summary>
    /// <param name="hostPath">The absolute path of a file or directory to mount on the host system.</param>
    /// <param name="containerPath">The absolute path of a file or directory to mount in the container.</param>
    /// <param name="accessMode">The Docker volume access mode.</param>
    public BindMount(string hostPath, string containerPath, AccessMode accessMode)
    {
      this.Type = MountType.Bind;
      this.Source = hostPath;
      this.Target = containerPath;
      this.AccessMode = accessMode;
    }

    public MountType Type { get; }

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public string Target { get; }

    /// <inheritdoc />
    public AccessMode AccessMode { get; }
  }
}
