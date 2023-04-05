namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using Docker.DotNet.Models;
  using JetBrains.Annotations;

  /// <summary>
  /// A network configuration.
  /// </summary>
  [PublicAPI]
  public interface INetworkConfiguration : IResourceConfiguration<NetworksCreateParameters>
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
    /// Gets a dictionary of network options.
    /// </summary>
    IReadOnlyDictionary<string, string> Options { get; }
  }
}
