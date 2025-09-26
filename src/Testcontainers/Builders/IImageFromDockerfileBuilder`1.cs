namespace DotNet.Testcontainers.Builders
{
  using System;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker image builder.
  /// </summary>
  /// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
  [PublicAPI]
  public interface IImageFromDockerfileBuilder<out TBuilderEntity>
  {
    /// <summary>
    /// Sets the name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithName(string name);

    /// <summary>
    /// Sets the name.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithName(IImage image);

    /// <summary>
    /// Sets the directory to use as the Docker build context.
    /// This is the folder that Docker will use to resolve files referenced in the Dockerfile.
    /// </summary>
    /// <param name="contextDirectory">An absolute path or relative name of the directory to use as the Docker build context.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithContextDirectory(string contextDirectory);

    /// <summary>
    /// Sets the path to the Dockerfile to use for the build.
    /// This can be an absolute path or a path relative to the Docker build context.
    /// </summary>
    /// <param name="dockerfile">The filename or path of the Dockerfile.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerfile(string dockerfile);

    /// <summary>
    /// Sets the directory containing the Dockerfile.
    /// This is useful if the Dockerfile is not located in the build context root.
    /// </summary>
    /// <param name="dockerfileDirectory">An absolute path or relative path to the directory containing the Dockerfile.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerfileDirectory(string dockerfileDirectory);

    /// <summary>
    /// Sets the directory containing the Dockerfile.
    /// This is useful if the Dockerfile is not located in the build context root.
    /// </summary>
    /// <param name="commonDirectoryPath">A common directory path that contains the Dockerfile directory.</param>
    /// <param name="dockerfileDirectory">An absolute path or relative path to the directory containing the Dockerfile.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerfileDirectory(CommonDirectoryPath commonDirectoryPath, string dockerfileDirectory);

    /// <summary>
    /// Sets the target build stage for the Docker image, allowing partial builds for
    /// multi-stage Dockerfiles.
    /// </summary>
    /// <param name="target">The target build stage to use for the image build.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithTarget(string target);

    /// <summary>
    /// Sets the image build policy.
    /// </summary>
    /// <param name="imageBuildPolicy">The image build policy.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithImageBuildPolicy(Func<ImageInspectResponse, bool> imageBuildPolicy);

    /// <summary>
    /// Removes an existing image before building it again.
    /// </summary>
    /// <param name="deleteIfExists">Determines whether Testcontainers removes an existing image or not.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDeleteIfExists(bool deleteIfExists);

    /// <summary>
    /// Sets the build argument.
    /// </summary>
    /// <param name="name">The build argument name.</param>
    /// <param name="value">The build argument value.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithBuildArgument(string name, string value);
  }
}
