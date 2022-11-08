namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Networks;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker network builder.
  /// </summary>
  [PublicAPI]
  public interface ITestcontainersNetworkBuilder : IAbstractBuilder<ITestcontainersNetworkBuilder>
  {
    /// <summary>
    /// Sets the name of the Docker network.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersNetworkBuilder" />.</returns>
    [PublicAPI]
    ITestcontainersNetworkBuilder WithName(string name);

    /// <summary>
    /// Sets the driver of the Docker network.
    /// </summary>
    /// <param name="driver">The driver.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersNetworkBuilder" />.</returns>
    [PublicAPI]
    ITestcontainersNetworkBuilder WithDriver(NetworkDriver driver);

    /// <summary>
    /// Sets the option of the Docker network.
    /// </summary>
    /// <param name="name">The option name.</param>
    /// <param name="value">The option value.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersNetworkBuilder" />.</returns>
    [PublicAPI]
    ITestcontainersNetworkBuilder WithOption(string name, string value);

    /// <summary>
    /// Builds the instance of <see cref="IDockerNetwork" /> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IDockerNetwork" />.</returns>
    [PublicAPI]
    IDockerNetwork Build();
  }
}
