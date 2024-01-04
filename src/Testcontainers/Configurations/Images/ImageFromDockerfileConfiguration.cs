namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Text.Json.Serialization;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IImageFromDockerfileConfiguration" />
  [PublicAPI]
  internal sealed class ImageFromDockerfileConfiguration : ResourceConfiguration<ImageBuildParameters>, IImageFromDockerfileConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="dockerfile">The Dockerfile.</param>
    /// <param name="dockerfileDirectory">The Dockerfile directory.</param>
    /// <param name="image">The image.</param>
    /// <param name="imageBuildPolicy">The image build policy.</param>
    /// <param name="buildArguments">A list of build arguments.</param>
    /// <param name="deleteIfExists">A value indicating whether Testcontainers removes an existing image or not.</param>
    public ImageFromDockerfileConfiguration(
      string dockerfile = null,
      string dockerfileDirectory = null,
      IImage image = null,
      Func<ImageInspectResponse, bool> imageBuildPolicy = null,
      IReadOnlyDictionary<string, string> buildArguments = null,
      bool? deleteIfExists = null)
    {
      Dockerfile = dockerfile;
      DockerfileDirectory = dockerfileDirectory;
      Image = image;
      ImageBuildPolicy = imageBuildPolicy;
      BuildArguments = buildArguments;
      DeleteIfExists = deleteIfExists;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ImageFromDockerfileConfiguration(IResourceConfiguration<ImageBuildParameters> resourceConfiguration)
      : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ImageFromDockerfileConfiguration(IImageFromDockerfileConfiguration resourceConfiguration)
      : this(new ImageFromDockerfileConfiguration(), resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ImageFromDockerfileConfiguration(IImageFromDockerfileConfiguration oldValue, IImageFromDockerfileConfiguration newValue)
      : base(oldValue, newValue)
    {
      Dockerfile = BuildConfiguration.Combine(oldValue.Dockerfile, newValue.Dockerfile);
      DockerfileDirectory = BuildConfiguration.Combine(oldValue.DockerfileDirectory, newValue.DockerfileDirectory);
      Image = BuildConfiguration.Combine(oldValue.Image, newValue.Image);
      ImageBuildPolicy = BuildConfiguration.Combine(oldValue.ImageBuildPolicy, newValue.ImageBuildPolicy);
      BuildArguments = BuildConfiguration.Combine(oldValue.BuildArguments, newValue.BuildArguments);
      DeleteIfExists = (oldValue.DeleteIfExists.HasValue && oldValue.DeleteIfExists.Value) || (newValue.DeleteIfExists.HasValue && newValue.DeleteIfExists.Value);
    }

    /// <inheritdoc />
    [JsonIgnore]
    public bool? DeleteIfExists { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public string Dockerfile { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public string DockerfileDirectory { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public IImage Image { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public Func<ImageInspectResponse, bool> ImageBuildPolicy { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public IReadOnlyDictionary<string, string> BuildArguments { get; }
  }
}
