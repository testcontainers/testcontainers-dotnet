namespace DotNet.Testcontainers.Containers.Configurations
{
  using System.IO;

  /// <inheritdoc cref="IBind" />
  internal readonly struct Mount : IBind
  {
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
