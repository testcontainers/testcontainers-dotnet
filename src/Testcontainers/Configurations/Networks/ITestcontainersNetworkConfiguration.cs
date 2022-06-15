namespace DotNet.Testcontainers.Configurations
{
  /// <summary>
  /// A Docker network configuration.
  /// </summary>
  public interface ITestcontainersNetworkConfiguration : IDockerResourceConfiguration
  {
    /// <summary>
    /// Gets the name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the driver.
    /// </summary>
    NetworkDriver Driver { get; }
  }
}
