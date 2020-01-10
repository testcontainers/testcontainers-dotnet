namespace DotNet.Testcontainers.Images.Builders
{
  using System.Threading.Tasks;

  public interface IImageFromDockerfileBuilder
  {
    /// <summary>
    /// Sets the name of the Docker image.
    /// </summary>
    /// <param name="name">Docker image name.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder"/>.</returns>
    IImageFromDockerfileBuilder WithName(string name);

    /// <summary>
    /// Sets the name of the Docker image.
    /// </summary>
    /// <param name="name">Docker image name.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder"/>.</returns>
    IImageFromDockerfileBuilder WithName(IDockerImage name);

    /// <summary>
    /// Sets the name of the Dockerfile.
    /// </summary>
    /// <param name="dockerfile">Dockerfile name.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder"/>.</returns>
    IImageFromDockerfileBuilder WithDockerfile(string dockerfile);

    /// <summary>
    /// Sets the base directory of the Dockerfile.
    /// </summary>
    /// <param name="dockerfileDirectory">Dockerfile base directory.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder"/>.</returns>
    IImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory);

    /// <summary>
    /// If true, Testcontainer will remove the existing Docker image. Otherwise, Testcontainer will keep the Docker image.
    /// </summary>
    /// <param name="deleteIfExists">True, Testcontainer will remove the Docker image. Otherwise, Testcontainer will keep it.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder"/>.</returns>
    IImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists);

    /// <summary>
    /// Builds the instance of <see cref="IImageFromDockerfileBuilder"/> with the given configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder"/>.</returns>
    Task<string> Build();
  }
}
