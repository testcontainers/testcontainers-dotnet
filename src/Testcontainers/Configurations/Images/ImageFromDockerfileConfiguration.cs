namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
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
    /// <param name="buildArguments">A list of build arguments.</param>
    /// <param name="deleteIfExists">A value indicating whether Testcontainers removes an existing image or not.</param>
    public ImageFromDockerfileConfiguration(
      string dockerfile = null,
      string dockerfileDirectory = null,
      IImage image = null,
      IReadOnlyDictionary<string, string> buildArguments = null,
      bool? deleteIfExists = null)
    {
      this.Dockerfile = dockerfile;
      this.DockerfileDirectory = dockerfileDirectory;
      this.Image = image;
      this.BuildArguments = buildArguments;
      this.DeleteIfExists = deleteIfExists;
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
      this.Dockerfile = BuildConfiguration.Combine(oldValue.Dockerfile, newValue.Dockerfile);
      this.DockerfileDirectory = BuildConfiguration.Combine(oldValue.DockerfileDirectory, newValue.DockerfileDirectory);
      this.Image = BuildConfiguration.Combine(oldValue.Image, newValue.Image);
      this.BuildArguments = BuildConfiguration.Combine(oldValue.BuildArguments, newValue.BuildArguments);
      this.DeleteIfExists = (oldValue.DeleteIfExists.HasValue && oldValue.DeleteIfExists.Value) || (newValue.DeleteIfExists.HasValue && newValue.DeleteIfExists.Value);
    }

    /// <inheritdoc />
    public bool? DeleteIfExists { get; }

    /// <inheritdoc />
    public string Dockerfile { get; }

    /// <inheritdoc />
    public string DockerfileDirectory { get; }

    /// <inheritdoc />
    public IImage Image { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> BuildArguments { get; }
  }
}
