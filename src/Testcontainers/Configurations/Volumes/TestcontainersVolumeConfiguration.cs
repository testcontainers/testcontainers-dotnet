namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;

  /// <inheritdoc cref="ITestcontainersVolumeConfiguration" />
  internal sealed class TestcontainersVolumeConfiguration : DockerResourceConfiguration, ITestcontainersVolumeConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersVolumeConfiguration" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    public TestcontainersVolumeConfiguration(IDockerResourceConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersVolumeConfiguration" /> class.
    /// </summary>
    /// <param name="dockerEndpointAuthenticationConfiguration">The Docker endpoint authentication configuration.</param>
    /// <param name="name">The name.</param>
    /// <param name="labels">A list of labels.</param>
    public TestcontainersVolumeConfiguration(
      IDockerEndpointAuthenticationConfiguration dockerEndpointAuthenticationConfiguration = null,
      string name = null,
      IReadOnlyDictionary<string, string> labels = null)
      : base(dockerEndpointAuthenticationConfiguration, labels)
    {
      this.Name = name;
    }

    /// <inheritdoc />
    public string Name { get; }
  }
}
