namespace DotNet.Testcontainers.Builders
{
  using System;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
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
    /// Sets the resource reaper session id.
    /// </summary>
    /// <param name="resourceReaperSessionId">The session id of the <see cref="ResourceReaper" /> instance.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    /// <remarks>The <see cref="ResourceReaper" /> will delete the resource after the tests has been finished.</remarks>
    [PublicAPI]
    ITestcontainersNetworkBuilder WithResourceReaperSessionId(Guid resourceReaperSessionId);

    /// <summary>
    /// Builds the instance of <see cref="IDockerNetwork" /> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IDockerNetwork" />.</returns>
    [PublicAPI]
    IDockerNetwork Build();
  }
}
