namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;

  /// <inheritdoc cref="ITestcontainersNetworkConfiguration" />
  internal sealed class TestcontainersNetworkConfiguration : DockerResourceConfiguration, ITestcontainersNetworkConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersNetworkConfiguration" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    public TestcontainersNetworkConfiguration(IDockerResourceConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersNetworkConfiguration" /> class.
    /// </summary>
    /// <param name="dockerEndpointAuthenticationConfiguration">The Docker endpoint authentication configuration.</param>
    /// <param name="name">The name.</param>
    /// <param name="driver">The driver.</param>
    /// <param name="labels">A list of labels.</param>
    public TestcontainersNetworkConfiguration(
      IDockerEndpointAuthenticationConfiguration dockerEndpointAuthenticationConfiguration = null,
      string name = null,
      NetworkDriver driver = default,
      IReadOnlyDictionary<string, string> labels = null)
      : base(dockerEndpointAuthenticationConfiguration, labels)
    {
      this.Name = name;
      this.Driver = driver;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public NetworkDriver Driver { get; }
  }
}
