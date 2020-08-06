namespace DotNet.Testcontainers.Containers.Configurations
{
  using System.IO;

  /// <inheritdoc cref="IBind" />
  internal readonly struct Mount : IBind
  {
    /// <summary>
    /// Creates a <see cref="Mount" />.
    /// </summary>
    /// <param name="hostPath">The absolute path of the directory to mount on the host system.</param>
    /// <param name="containerPath">The absolute path of the directory to mount in the container.</param>
    /// <param name="accessMode">The Docker volume access mode.</param>
    public Mount(string hostPath, string containerPath, AccessMode accessMode)
    {
      this.HostPath = Path.GetFullPath(hostPath);
      this.ContainerPath = containerPath;
      this.AccessMode = accessMode;
    }

    /// <inheritdoc />
    public string HostPath { get; }

    /// <inheritdoc />
    public string ContainerPath { get; }

    /// <inheritdoc />
    public AccessMode AccessMode { get; }
  }
}
