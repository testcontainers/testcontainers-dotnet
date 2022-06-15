namespace DotNet.Testcontainers.Builders
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker image builder.
  /// </summary>
  [PublicAPI]
  public interface IImageFromDockerfileBuilder : IAbstractBuilder<IImageFromDockerfileBuilder>
  {
    /// <summary>
    /// Sets the name of the Docker image.
    /// </summary>
    /// <param name="name">Docker image name.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    [PublicAPI]
    IImageFromDockerfileBuilder WithName(string name);

    /// <summary>
    /// Sets the name of the Docker image.
    /// </summary>
    /// <param name="name">Docker image name.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    [PublicAPI]
    IImageFromDockerfileBuilder WithName(IDockerImage name);

    /// <summary>
    /// Sets the name of the Dockerfile.
    /// </summary>
    /// <param name="dockerfile">Dockerfile name.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    [PublicAPI]
    IImageFromDockerfileBuilder WithDockerfile(string dockerfile);

    /// <summary>
    /// Sets the base directory of the Dockerfile.
    /// </summary>
    /// <param name="dockerfileDirectory">Dockerfile base directory.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    [PublicAPI]
    IImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory);

    /// <summary>
    /// If true, Testcontainer will remove the existing Docker image. Otherwise, Testcontainer will keep the Docker image.
    /// </summary>
    /// <param name="deleteIfExists">True, Testcontainer will remove the Docker image. Otherwise, Testcontainer will keep it.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    [PublicAPI]
    IImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists);

    /// <summary>
    /// Builds the instance of <see cref="IImageFromDockerfileBuilder" /> with the given configuration.
    /// </summary>
    /// <returns>FullName of the created image.</returns>
    [PublicAPI]
    Task<string> Build();
  }
}
