namespace DotNet.Testcontainers.Configurations
{
  using System.IO;

  /// <inheritdoc cref="IBindMount" />
  internal readonly struct BindMount : IBindMount
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BindMount" /> struct.
    /// </summary>
    /// <param name="hostPath">The absolute path of a file or directory to mount on the host system.</param>
    /// <param name="containerPath">The absolute path of a file or directory to mount in the container.</param>
    /// <param name="accessMode">The Docker volume access mode.</param>
    public BindMount(string hostPath, string containerPath, AccessMode accessMode)
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
