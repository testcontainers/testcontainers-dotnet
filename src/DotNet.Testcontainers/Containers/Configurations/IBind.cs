namespace DotNet.Testcontainers.Containers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// Binding for Docker volumes.
  /// </summary>
  public interface IBind
  {
    /// <summary>
    /// Gets the absolute path of the directory to mount on the host system.
    /// </summary>
    [NotNull]
    string HostPath { get; }

    /// <summary>
    /// Gets the absolute path of the directory to mount in the container.
    /// </summary>
    [NotNull]
    string ContainerPath { get; }

    /// <summary>
    /// Gets the Docker volume access mode.
    /// </summary>
    AccessMode AccessMode { get; }
  }
}
