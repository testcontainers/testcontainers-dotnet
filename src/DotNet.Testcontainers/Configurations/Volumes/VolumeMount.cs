namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Volumes;

  /// <inheritdoc cref="IVolumeMount" />
  internal readonly struct VolumeMount : IVolumeMount
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeMount" /> struct.
    /// </summary>
    /// <param name="volume">The volume to mount on the host system.</param>
    /// <param name="containerPath">The absolute path of a file or directory to mount in the container.</param>
    /// <param name="accessMode">The Docker volume access mode.</param>
    public VolumeMount(IDockerVolume volume, string containerPath, AccessMode accessMode)
    {
      this.Volume = volume;
      this.ContainerPath = containerPath;
      this.AccessMode = accessMode;
    }

    /// <inheritdoc />
    public IDockerVolume Volume { get; }

    /// <inheritdoc />
    public string ContainerPath { get; }

    /// <inheritdoc />
    public AccessMode AccessMode { get; }
  }
}
