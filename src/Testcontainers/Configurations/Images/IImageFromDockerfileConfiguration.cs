namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// An image configuration.
  /// </summary>
  [PublicAPI]
  public interface IImageFromDockerfileConfiguration : IResourceConfiguration<ImageBuildParameters>
  {
    /// <summary>
    /// Gets a value indicating whether Testcontainers removes an existing image or not.
    /// </summary>
    bool? DeleteIfExists { get; }

    /// <summary>
    /// Gets the Dockerfile.
    /// </summary>
    string Dockerfile { get; }

    /// <summary>
    /// Gets the Dockerfile directory.
    /// </summary>
    string DockerfileDirectory { get; }

    /// <summary>
    /// Gets the image.
    /// </summary>
    IImage Image { get; }

    /// <summary>
    /// Gets the image build policy.
    /// </summary>
    Func<ImageInspectResponse, bool> ImageBuildPolicy { get; }

    /// <summary>
    /// Gets a list of build arguments.
    /// </summary>
    IReadOnlyDictionary<string, string> BuildArguments { get; }
  }
}
