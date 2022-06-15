namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Volumes;

  /// <inheritdoc cref="IMount" />
  internal readonly struct VolumeMount : IMount
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeMount" /> struct.
    /// </summary>
    /// <param name="volume">The volume to mount on the host system.</param>
    /// <param name="containerPath">The absolute path of a file or directory to mount in the container.</param>
    /// <param name="accessMode">The Docker volume access mode.</param>
    public VolumeMount(IDockerVolume volume, string containerPath, AccessMode accessMode)
    {
      this.Type = MountType.Volume;
      this.Source = volume.Name;
      this.Target = containerPath;
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
