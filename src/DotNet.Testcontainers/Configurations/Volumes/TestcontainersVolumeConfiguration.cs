namespace DotNet.Testcontainers.Configurations.Volumes
{
  using System;
  using System.Collections.Generic;

  /// <inheritdoc cref="ITestcontainersVolumeConfiguration" />
  internal sealed class TestcontainersVolumeConfiguration : ITestcontainersVolumeConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersVolumeConfiguration" /> class.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <param name="name">The name.</param>
    /// <param name="labels">A list of labels.</param>
    public TestcontainersVolumeConfiguration(
      Uri endpoint,
      string name,
      IReadOnlyDictionary<string, string> labels)
    {
      this.Endpoint = endpoint;
      this.Name = name;
      this.Labels = labels;
    }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Labels { get; }
  }
}
