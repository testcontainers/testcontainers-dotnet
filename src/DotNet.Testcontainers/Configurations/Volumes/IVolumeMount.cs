namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Volumes;
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents a volume mount.
  /// </summary>
  public interface IVolumeMount : IMount
  {
    /// <summary>
    /// Gets the volume to mount on the host system.
    /// </summary>
    [NotNull]
    IDockerVolume Volume { get; }
  }
}
