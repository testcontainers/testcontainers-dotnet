namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Network;
  using JetBrains.Annotations;

  /// <inheritdoc cref="ITestcontainersNetworkBuilder" />
  [PublicAPI]
  public sealed class TestcontainersNetworkBuilder : ITestcontainersNetworkBuilder
  {
    private readonly ITestcontainersNetworkConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersNetworkBuilder" /> class.
    /// </summary>
    public TestcontainersNetworkBuilder()
      : this(
        Apply(
          endpoint: TestcontainersSettings.OS.DockerApiEndpoint,
          labels: DefaultLabels.Instance,
          driver: NetworkDriver.Bridge))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersNetworkBuilder" /> class.
    /// </summary>
    /// <param name="configuration">The Docker network configuration.</param>
    private TestcontainersNetworkBuilder(ITestcontainersNetworkConfiguration configuration)
    {
      this.configuration = configuration;
    }

    /// <inheritdoc />
    public ITestcontainersNetworkBuilder WithDockerEndpoint(string endpoint)
    {
      return Build(this, Apply(endpoint: new Uri(endpoint)));
    }

    /// <inheritdoc />
    public ITestcontainersNetworkBuilder WithDriver(NetworkDriver driver)
    {
      return Build(this, Apply(driver: driver));
    }

    /// <inheritdoc />
    public ITestcontainersNetworkBuilder WithName(string name)
    {
      return Build(this, Apply(name: name));
    }

    /// <inheritdoc />
    public ITestcontainersNetworkBuilder WithLabel(string name, string value)
    {
      var labels = new Dictionary<string, string> { { name, value } };
      return Build(this, Apply(labels: labels));
    }

    /// <inheritdoc />
    public IDockerNetwork Build()
    {
      return new NonExistingDockerNetwork(this.configuration, TestcontainersSettings.Logger);
    }

    private static ITestcontainersNetworkConfiguration Apply(
      Uri endpoint = null,
      NetworkDriver driver = default,
      string name = null,
      IReadOnlyDictionary<string, string> labels = null)
    {
      return new TestcontainersNetworkConfiguration(
        endpoint,
        driver,
        name,
        labels);
    }

    private static ITestcontainersNetworkBuilder Build(
      TestcontainersNetworkBuilder previous,
      ITestcontainersNetworkConfiguration next)
    {
      var endpoint = BuildConfiguration.Combine(next.Endpoint, previous.configuration.Endpoint);
      var driver = BuildConfiguration.Combine(next.Driver, previous.configuration.Driver);
      var name = BuildConfiguration.Combine(next.Name, previous.configuration.Name);
      var labels = BuildConfiguration.Combine(next.Labels, previous.configuration.Labels);

      var mergedConfiguration = Apply(
        endpoint,
        driver,
        name,
        labels);

      return new TestcontainersNetworkBuilder(mergedConfiguration);
    }
  }
}
