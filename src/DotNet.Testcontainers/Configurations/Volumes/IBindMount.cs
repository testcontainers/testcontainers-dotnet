namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents a binding of a file or directory.
  /// </summary>
  public interface IBindMount
  {
    /// <summary>
    /// Gets the absolute path of a file or directory to mount on the host system.
    /// </summary>
    [NotNull]
    string HostPath { get; }

    /// <summary>
    /// Gets the absolute path of a file or directory to mount in the container.
    /// </summary>
    [NotNull]
    string ContainerPath { get; }

    /// <summary>
    /// Gets the Docker mount access mode.
    /// </summary>
    AccessMode AccessMode { get; }
  }
}
