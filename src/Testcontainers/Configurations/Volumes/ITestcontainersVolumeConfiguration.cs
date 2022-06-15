namespace DotNet.Testcontainers.Configurations
{
  /// <summary>
  /// A Docker volume configuration.
  /// </summary>
  public interface ITestcontainersVolumeConfiguration : IDockerResourceConfiguration
  {
    /// <summary>
    /// Gets the name.
    /// </summary>
    string Name { get; }
  }
}
