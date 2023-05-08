namespace DotNet.Testcontainers.Configurations
{
  using Docker.DotNet.Models;
  using JetBrains.Annotations;

  /// <summary>
  /// A volume configuration.
  /// </summary>
  [PublicAPI]
  public interface IVolumeConfiguration : IResourceConfiguration<VolumesCreateParameters>
  {
    /// <summary>
    /// Gets the name.
    /// </summary>
    string Name { get; }
  }
}
