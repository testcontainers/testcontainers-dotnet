namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;

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

    /// <summary>
    /// Gets the additional options.
    /// </summary>
    IReadOnlyDictionary<string, string> Options { get; }
  }
}
