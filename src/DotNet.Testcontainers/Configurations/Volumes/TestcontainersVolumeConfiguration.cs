namespace DotNet.Testcontainers.Configurations.Volumes
{
  using System;

  /// <inheritdoc cref="ITestcontainersVolumeConfiguration" />
  internal sealed class TestcontainersVolumeConfiguration : ITestcontainersVolumeConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersVolumeConfiguration" /> class.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <param name="name">The name.</param>
    public TestcontainersVolumeConfiguration(
      Uri endpoint,
      string name)
    {
      this.Endpoint = endpoint;
      this.Name = name;
    }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public string Name { get; }
  }
}
