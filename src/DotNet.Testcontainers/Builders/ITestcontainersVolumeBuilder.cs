namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Volumes;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker volume builder.
  /// </summary>
  public interface ITestcontainersVolumeBuilder
  {
    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersVolumeBuilder" />.</returns>
    [PublicAPI]
    ITestcontainersVolumeBuilder WithDockerEndpoint(string endpoint);

    /// <summary>
    /// Sets the name of the Docker volume.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersVolumeBuilder" />.</returns>
    [PublicAPI]
    ITestcontainersVolumeBuilder WithName(string name);

    /// <summary>
    /// Adds user-defined metadata to the Docker volume.
    /// </summary>
    /// <param name="name">Label name.</param>
    /// <param name="value">Label value.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersVolumeBuilder" />.</returns>
    [PublicAPI]
    ITestcontainersVolumeBuilder WithLabel(string name, string value);

    /// <summary>
    /// Builds the instance of <see cref="IDockerVolume" /> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IDockerVolume" />.</returns>
    [PublicAPI]
    IDockerVolume Build();
  }
}
