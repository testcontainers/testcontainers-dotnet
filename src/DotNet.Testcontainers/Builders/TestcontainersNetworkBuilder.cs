namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Networks;

  /// <inheritdoc cref="ITestcontainersNetworkBuilder" />
  public class TestcontainersNetworkBuilder : AbstractBuilder<ITestcontainersNetworkBuilder, ITestcontainersNetworkConfiguration>, ITestcontainersNetworkBuilder
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersNetworkBuilder" /> class.
    /// </summary>
    public TestcontainersNetworkBuilder()
      : this(new TestcontainersNetworkConfiguration(
        endpoint: TestcontainersSettings.OS.DockerApiEndpoint,
        labels: DefaultLabels.Instance))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersNetworkBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker network configuration.</param>
    private TestcontainersNetworkBuilder(ITestcontainersNetworkConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
    }

    /// <inheritdoc />
    public ITestcontainersNetworkBuilder WithName(string name)
    {
      return this.MergeNewConfiguration(new TestcontainersNetworkConfiguration(name: name));
    }

    /// <inheritdoc />
    public ITestcontainersNetworkBuilder WithDriver(NetworkDriver driver)
    {
      return this.MergeNewConfiguration(new TestcontainersNetworkConfiguration(driver: driver));
    }

    /// <inheritdoc />
    public IDockerNetwork Build()
    {
      return new NonExistingDockerNetwork(this.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override ITestcontainersNetworkBuilder MergeNewConfiguration(IDockerResourceConfiguration dockerResourceConfiguration)
    {
      return this.MergeNewConfiguration(new TestcontainersNetworkConfiguration(dockerResourceConfiguration));
    }

    /// <summary>
    /// Merges the current with the new Docker resource configuration.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The new Docker resource configuration.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersNetworkBuilder" />.</returns>
    protected virtual ITestcontainersNetworkBuilder MergeNewConfiguration(ITestcontainersNetworkConfiguration dockerResourceConfiguration)
    {
      var endpoint = BuildConfiguration.Combine(dockerResourceConfiguration.Endpoint, this.DockerResourceConfiguration.Endpoint);
      var name = BuildConfiguration.Combine(dockerResourceConfiguration.Name, this.DockerResourceConfiguration.Name);
      var driver = BuildConfiguration.Combine(dockerResourceConfiguration.Driver, this.DockerResourceConfiguration.Driver);
      var labels = BuildConfiguration.Combine(dockerResourceConfiguration.Labels, this.DockerResourceConfiguration.Labels);
      return new TestcontainersNetworkBuilder(new TestcontainersNetworkConfiguration(endpoint, name, driver, labels));
    }
  }
}
