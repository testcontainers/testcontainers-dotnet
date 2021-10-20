namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Volumes;
  using JetBrains.Annotations;

  /// <inheritdoc cref="ITestcontainersVolumeBuilder" />
  [PublicAPI]
  public sealed class TestcontainersVolumeBuilder : ITestcontainersVolumeBuilder
  {
    private readonly ITestcontainersVolumeConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersVolumeBuilder" /> class.
    /// </summary>
    public TestcontainersVolumeBuilder()
      : this(
        Apply(
          endpoint: TestcontainersSettings.OS.DockerApiEndpoint,
          labels: DefaultLabels.Instance))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersVolumeBuilder" /> class.
    /// </summary>
    /// <param name="configuration">The Docker volume configuration.</param>
    private TestcontainersVolumeBuilder(ITestcontainersVolumeConfiguration configuration)
    {
      this.configuration = configuration;
    }

    /// <inheritdoc />
    public ITestcontainersVolumeBuilder WithDockerEndpoint(string endpoint)
    {
      return Build(this, Apply(endpoint: new Uri(endpoint)));
    }

    /// <inheritdoc />
    public ITestcontainersVolumeBuilder WithName(string name)
    {
      return Build(this, Apply(name: name));
    }

    /// <inheritdoc />
    public ITestcontainersVolumeBuilder WithLabel(string name, string value)
    {
      var labels = new Dictionary<string, string> { { name, value } };
      return Build(this, Apply(labels: labels));
    }

    /// <inheritdoc />
    public IDockerVolume Build()
    {
      return new NonExistingDockerVolume(this.configuration, TestcontainersSettings.Logger);
    }

    private static ITestcontainersVolumeConfiguration Apply(
      Uri endpoint = null,
      string name = null,
      IReadOnlyDictionary<string, string> labels = null)
    {
      return new TestcontainersVolumeConfiguration(endpoint, name, labels);
    }

    private static ITestcontainersVolumeBuilder Build(
      TestcontainersVolumeBuilder previous,
      ITestcontainersVolumeConfiguration next)
    {
      var endpoint = BuildConfiguration.Combine(next.Endpoint, previous.configuration.Endpoint);
      var name = BuildConfiguration.Combine(next.Name, previous.configuration.Name);
      var labels = BuildConfiguration.Combine(next.Labels, previous.configuration.Labels);

      var mergedConfiguration = Apply(
        endpoint,
        name,
        labels);

      return new TestcontainersVolumeBuilder(mergedConfiguration);
    }
  }
}
