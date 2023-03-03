namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Volumes;

  /// <inheritdoc cref="IMount" />
  internal readonly struct VolumeMount : IMount
  {
    // Do not set the volume name immediately in the constructor. This may raise an InvalidOperationException too early.
    // Depending on when the developer passes the instance to VolumeMount it may not exist yet.
    private readonly IVolume volume;

    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeMount" /> struct.
    /// </summary>
    /// <param name="volume">The volume to mount on the host system.</param>
    /// <param name="containerPath">The absolute path of a file or directory to mount in the container.</param>
    /// <param name="accessMode">The Docker volume access mode.</param>
    public VolumeMount(IVolume volume, string containerPath, AccessMode accessMode)
    {
      this.volume = volume;
      this.Type = MountType.Volume;
      this.Target = containerPath;
      this.AccessMode = accessMode;
    }

    /// <inheritdoc />
    public MountType Type { get; }

    /// <inheritdoc />
    public AccessMode AccessMode { get; }

    /// <inheritdoc />
    public string Source => this.volume.Name;

    /// <inheritdoc />
    public string Target { get; }
  }
}
