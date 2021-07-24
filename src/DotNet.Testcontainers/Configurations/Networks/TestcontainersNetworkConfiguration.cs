namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;

  /// <inheritdoc cref="ITestcontainersNetworkConfiguration" />
  internal sealed class TestcontainersNetworkConfiguration : ITestcontainersNetworkConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersNetworkConfiguration" /> class.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <param name="name">The name.</param>
    /// <param name="driver">The driver.</param>
    /// <param name="labels">A list of labels.</param>
    public TestcontainersNetworkConfiguration(
      Uri endpoint,
      NetworkDriver driver,
      string name,
      IReadOnlyDictionary<string, string> labels)
    {
      this.Endpoint = endpoint;
      this.Name = name;
      this.Driver = driver;
      this.Labels = labels;
    }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public NetworkDriver Driver { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Labels { get; }
  }
}
