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
    /// <param name="options">A list of additional options.</param>
    public TestcontainersNetworkConfiguration(
      IDockerEndpointAuthenticationConfiguration dockerEndpointAuthenticationConfiguration = null,
      string name = null,
      NetworkDriver driver = default,
      IReadOnlyDictionary<string, string> labels = null,
      IReadOnlyDictionary<string, string> options = null)
      : base(dockerEndpointAuthenticationConfiguration, labels)
    {
      this.Options = options;
      this.Name = name;
      this.Driver = driver;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public NetworkDriver Driver { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Options { get; }
  }
}
