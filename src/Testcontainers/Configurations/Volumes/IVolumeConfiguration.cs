namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// A volume configuration.
  /// </summary>
  [PublicAPI]
  public interface IVolumeConfiguration : IResourceConfiguration
  {
    /// <summary>
    /// Gets the name.
    /// </summary>
    string Name { get; }
  }
}
