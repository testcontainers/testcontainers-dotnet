namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Network;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker network builder.
  /// </summary>
  public interface ITestcontainersNetworkBuilder
  {
    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersNetworkBuilder" />.</returns>
    [PublicAPI]
    ITestcontainersNetworkBuilder WithDockerEndpoint(string endpoint);

    /// <summary>
    /// Sets the driver of the Docker network.
    /// </summary>
    /// <param name="driver">The driver.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersNetworkBuilder" />.</returns>
    [PublicAPI]
    ITestcontainersNetworkBuilder WithDriver(NetworkDriver driver);

    /// <summary>
    /// Sets the name of the Docker network.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersNetworkBuilder" />.</returns>
    [PublicAPI]
    ITestcontainersNetworkBuilder WithName(string name);

    /// <summary>
    /// Adds user-defined metadata to the Docker network.
    /// </summary>
    /// <param name="name">Label name.</param>
    /// <param name="value">Label value.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersNetworkBuilder" />.</returns>
    [PublicAPI]
    ITestcontainersNetworkBuilder WithLabel(string name, string value);

    /// <summary>
    /// Builds the instance of <see cref="IDockerNetwork" /> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IDockerNetwork" />.</returns>
    [PublicAPI]
    IDockerNetwork Build();
  }
}
