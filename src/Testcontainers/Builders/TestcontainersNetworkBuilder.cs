namespace DotNet.Testcontainers.Builders
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Networks;
  using JetBrains.Annotations;

  /// <inheritdoc cref="ITestcontainersNetworkBuilder" />
  [PublicAPI]
  public class TestcontainersNetworkBuilder : AbstractBuilder<ITestcontainersNetworkBuilder, ITestcontainersNetworkConfiguration>, ITestcontainersNetworkBuilder
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersNetworkBuilder" /> class.
    /// </summary>
    public TestcontainersNetworkBuilder()
      : this(new TestcontainersNetworkConfiguration(
        dockerEndpointAuthenticationConfiguration: TestcontainersSettings.OS.DockerEndpointAuthConfig,
        labels: DefaultLabels.Instance,
        options: new ReadOnlyDictionary<string, string>(new Dictionary<string, string>())))
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
    public ITestcontainersNetworkBuilder WithOption(string name, string value)
    {
      var options = new Dictionary<string, string> { { name, value } };
      return this.MergeNewConfiguration(new TestcontainersNetworkConfiguration(options: options));
    }

    /// <inheritdoc />
    public IDockerNetwork Build()
    {
      _ = Guard.Argument(this.DockerResourceConfiguration.DockerEndpointAuthConfig, nameof(IDockerResourceConfiguration.DockerEndpointAuthConfig))
        .DockerEndpointAuthConfigIsSet();

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
      var dockerEndpointAuthConfig = BuildConfiguration.Combine(dockerResourceConfiguration.DockerEndpointAuthConfig, this.DockerResourceConfiguration.DockerEndpointAuthConfig);
      var name = BuildConfiguration.Combine(dockerResourceConfiguration.Name, this.DockerResourceConfiguration.Name);
      var driver = BuildConfiguration.Combine(dockerResourceConfiguration.Driver, this.DockerResourceConfiguration.Driver);
      var labels = BuildConfiguration.Combine(dockerResourceConfiguration.Labels, this.DockerResourceConfiguration.Labels);
      var options = BuildConfiguration.Combine(dockerResourceConfiguration.Options, this.DockerResourceConfiguration.Options);
      return new TestcontainersNetworkBuilder(new TestcontainersNetworkConfiguration(dockerEndpointAuthConfig, name, driver, labels, options));
    }
  }
}
