namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker image builder.
  /// </summary>
  /// <example>
  ///   The default configuration is equivalent to:
  ///   <code>
  ///   _ = new ImageBuilder()
  ///     .WithDockerEndpoint(TestcontainersSettings.OS.DockerEndpointAuthConfig)
  ///     .WithLabel(DefaultLabels.Instance)
  ///     .WithCleanUp(true)
  ///     .WithDockerfile("Dockerfile")
  ///     .WithDockerfileDirectory(Directory.GetCurrentDirectory())
  ///     .WithName(new DockerImage($"testcontainers:{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"))
  ///     .Build();
  ///   </code>
  /// </example>
  [PublicAPI]
  public class ImageBuilder : AbstractBuilder<ImageBuilder, IImage, IImageConfiguration>, IImageBuilder<ImageBuilder>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageBuilder" /> class.
    /// </summary>
    public ImageBuilder()
      : this(new ImageConfiguration())
    {
      this.DockerResourceConfiguration = this.Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private ImageBuilder(IImageConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
      this.DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override IImageConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public ImageBuilder WithName(string name)
    {
      return this.WithName(new DockerImage(name));
    }

    /// <inheritdoc />
    public ImageBuilder WithName(IImage name)
    {
      return this.Clone(new ImageConfiguration(image: name));
    }

    /// <inheritdoc />
    public ImageBuilder WithDockerfile(string dockerfile)
    {
      return this.Clone(new ImageConfiguration(dockerfile: dockerfile));
    }

    /// <inheritdoc />
    public ImageBuilder WithDockerfileDirectory(string dockerfileDirectory)
    {
      return this.Clone(new ImageConfiguration(dockerfileDirectory: dockerfileDirectory));
    }

    /// <inheritdoc />
    public ImageBuilder WithDockerfileDirectory(CommonDirectoryPath commonDirectoryPath, string dockerfileDirectory)
    {
      var baseDirectoryPath = Path.Combine(commonDirectoryPath.DirectoryPath, dockerfileDirectory);
      return this.Clone(new ImageConfiguration(dockerfileDirectory: baseDirectoryPath));
    }

    /// <inheritdoc />
    public ImageBuilder WithDeleteIfExists(bool deleteIfExists)
    {
      return this.Clone(new ImageConfiguration(deleteIfExists: deleteIfExists));
    }

    /// <inheritdoc />
    public ImageBuilder WithBuildArgument(string name, string value)
    {
      var buildArguments = new Dictionary<string, string> { { name, value } };
      return this.Clone(new ImageConfiguration(buildArguments: buildArguments));
    }

    public ImageBuilder WithName(IDockerImage image)
    {
      return this.WithName((IImage)image);
    }

    /// <inheritdoc />
    public override IImage Build()
    {
      this.Validate();
      throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected sealed override ImageBuilder Init()
    {
      return base.Init().WithDockerfile("Dockerfile").WithDockerfileDirectory(Directory.GetCurrentDirectory()).WithName(new DockerImage($"testcontainers:{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"));
    }

    /// <inheritdoc />
    protected override ImageBuilder Clone(IResourceConfiguration resourceConfiguration)
    {
      return this.Merge(this.DockerResourceConfiguration, new ImageConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ImageBuilder Merge(IImageConfiguration oldValue, IImageConfiguration newValue)
    {
      return new ImageBuilder(new ImageConfiguration(oldValue, newValue));
    }
  }
}
