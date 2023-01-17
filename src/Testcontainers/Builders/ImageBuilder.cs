namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
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
  ///     .WithName(new DockerImage(string.Empty, "testcontainers", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)))
  ///     .Build();
  ///   </code>
  /// </example>
  [PublicAPI]
  public class ImageBuilder : AbstractBuilder<ImageBuilder, IFutureDockerImage, IImageConfiguration>, IImageBuilder<ImageBuilder>
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
      return this.Merge(this.DockerResourceConfiguration, new ImageConfiguration(image: name));
    }

    /// <inheritdoc />
    public ImageBuilder WithDockerfile(string dockerfile)
    {
      return this.Merge(this.DockerResourceConfiguration, new ImageConfiguration(dockerfile: dockerfile));
    }

    /// <inheritdoc />
    public ImageBuilder WithDockerfileDirectory(string dockerfileDirectory)
    {
      return this.Merge(this.DockerResourceConfiguration, new ImageConfiguration(dockerfileDirectory: dockerfileDirectory));
    }

    /// <inheritdoc />
    public ImageBuilder WithDockerfileDirectory(CommonDirectoryPath commonDirectoryPath, string dockerfileDirectory)
    {
      var dockerfileDirectoryPath = Path.Combine(commonDirectoryPath.DirectoryPath, dockerfileDirectory);
      return this.Merge(this.DockerResourceConfiguration, new ImageConfiguration(dockerfileDirectory: dockerfileDirectoryPath));
    }

    /// <inheritdoc />
    public ImageBuilder WithDeleteIfExists(bool deleteIfExists)
    {
      return this.Merge(this.DockerResourceConfiguration, new ImageConfiguration(deleteIfExists: deleteIfExists));
    }

    /// <inheritdoc />
    public ImageBuilder WithBuildArgument(string name, string value)
    {
      var buildArguments = new Dictionary<string, string> { { name, value } };
      return this.Merge(this.DockerResourceConfiguration, new ImageConfiguration(buildArguments: buildArguments));
    }

    public ImageBuilder WithName(IDockerImage image)
    {
      return this.WithName((IImage)image);
    }

    /// <inheritdoc />
    public override IFutureDockerImage Build()
    {
      this.Validate();
      return new FutureDockerImage(this.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected sealed override ImageBuilder Init()
    {
      return base.Init().WithDockerfile("Dockerfile").WithDockerfileDirectory(Directory.GetCurrentDirectory()).WithName(new DockerImage(string.Empty, "testcontainers", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)));
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
