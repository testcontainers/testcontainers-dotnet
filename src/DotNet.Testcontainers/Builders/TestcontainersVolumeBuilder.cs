namespace DotNet.Testcontainers.Builders
{
  using System;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Configurations.Volumes;
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
      : this(Apply(endpoint: TestcontainersSettings.OS.DockerApiEndpoint))
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
    public ITestcontainersVolumeBuilder WithName(string name)
    {
      return Build(this, Apply(name: name));
    }

    /// <inheritdoc />
    public IDockerVolume Build()
    {
      return new NonExistingDockerVolume(this.configuration, TestcontainersSettings.Logger);
    }

    private static ITestcontainersVolumeConfiguration Apply(Uri endpoint = null, string name = null)
    {
      return new TestcontainersVolumeConfiguration(endpoint, name);
    }

    private static ITestcontainersVolumeBuilder Build(
      TestcontainersVolumeBuilder previous,
      ITestcontainersVolumeConfiguration next)
    {
      var endpoint = BuildConfiguration.Combine(next.Endpoint, previous.configuration.Endpoint);
      var name = BuildConfiguration.Combine(next.Name, previous.configuration.Name);

      var mergedConfiguration = Apply(
        endpoint,
        name);

      return new TestcontainersVolumeBuilder(mergedConfiguration);
    }
  }
}
