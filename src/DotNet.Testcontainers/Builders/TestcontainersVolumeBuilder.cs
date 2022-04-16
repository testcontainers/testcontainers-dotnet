namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Volumes;

  /// <inheritdoc cref="ITestcontainersVolumeBuilder" />
  public class TestcontainersVolumeBuilder : AbstractBuilder<ITestcontainersVolumeBuilder, ITestcontainersVolumeConfiguration>, ITestcontainersVolumeBuilder
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersVolumeBuilder" /> class.
    /// </summary>
    public TestcontainersVolumeBuilder()
      : this(new TestcontainersVolumeConfiguration(
        endpoint: TestcontainersSettings.OS.DockerApiEndpoint,
        labels: DefaultLabels.Instance))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersVolumeBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker volume configuration.</param>
    private TestcontainersVolumeBuilder(ITestcontainersVolumeConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
    }

    /// <inheritdoc />
    public ITestcontainersVolumeBuilder WithName(string name)
    {
      return this.MergeNewConfiguration(new TestcontainersVolumeConfiguration(name: name));
    }

    /// <inheritdoc />
    public IDockerVolume Build()
    {
      return new NonExistingDockerVolume(this.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override ITestcontainersVolumeBuilder MergeNewConfiguration(IDockerResourceConfiguration dockerResourceConfiguration)
    {
      return this.MergeNewConfiguration(new TestcontainersVolumeConfiguration(dockerResourceConfiguration));
    }

    /// <summary>
    /// Merges the current with the new Docker resource configuration.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The new Docker resource configuration.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersVolumeBuilder" />.</returns>
    protected virtual ITestcontainersVolumeBuilder MergeNewConfiguration(ITestcontainersVolumeConfiguration dockerResourceConfiguration)
    {
      var endpoint = BuildConfiguration.Combine(dockerResourceConfiguration.Endpoint, this.DockerResourceConfiguration.Endpoint);
      var name = BuildConfiguration.Combine(dockerResourceConfiguration.Name, this.DockerResourceConfiguration.Name);
      var labels = BuildConfiguration.Combine(dockerResourceConfiguration.Labels, this.DockerResourceConfiguration.Labels);
      return new TestcontainersVolumeBuilder(new TestcontainersVolumeConfiguration(endpoint, name, labels));
    }
  }
}
