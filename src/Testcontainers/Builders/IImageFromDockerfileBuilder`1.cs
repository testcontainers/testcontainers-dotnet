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
    /// <param name="name">The name.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithName(IImage name);

    /// <summary>
    /// Sets the Dockerfile.
    /// </summary>
    /// <param name="dockerfile">An absolute path or a name value within the Docker build context.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerfile(string dockerfile);

    /// <summary>
    /// Sets the Dockerfile directory.
    /// </summary>
    /// <param name="dockerfileDirectory">An absolute path or a name value to the Docker build context.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerfileDirectory(string dockerfileDirectory);

    /// <summary>
    /// Sets the Dockerfile directory.
    /// </summary>
    /// <param name="commonDirectoryPath">A common directory path that contains the Dockerfile directory.</param>
    /// <param name="dockerfileDirectory">A relative path or a name value to the Docker build context.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerfileDirectory(CommonDirectoryPath commonDirectoryPath, string dockerfileDirectory);

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
