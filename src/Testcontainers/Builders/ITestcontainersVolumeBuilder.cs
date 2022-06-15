namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Volumes;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker volume builder.
  /// </summary>
  [PublicAPI]
  public interface ITestcontainersVolumeBuilder : IAbstractBuilder<ITestcontainersVolumeBuilder>
  {
    /// <summary>
    /// Sets the name of the Docker volume.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A configured instance of <see cref="ITestcontainersVolumeBuilder" />.</returns>
    [PublicAPI]
    ITestcontainersVolumeBuilder WithName(string name);

    /// <summary>
    /// Builds the instance of <see cref="IDockerVolume" /> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IDockerVolume" />.</returns>
    [PublicAPI]
    IDockerVolume Build();
  }
}
