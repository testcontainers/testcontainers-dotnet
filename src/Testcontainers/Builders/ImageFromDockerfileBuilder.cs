namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker image builder.
  /// </summary>
  /// <example>
  ///   The default configuration is equivalent to:
  ///   <code>
  ///   _ = new ImageFromDockerfileBuilder()
  ///     .WithDockerEndpoint(TestcontainersSettings.OS.DockerEndpointAuthConfig)
  ///     .WithLabel(DefaultLabels.Instance)
  ///     .WithCleanUp(true)
  ///     .WithDockerfile("Dockerfile")
  ///     .WithDockerfileDirectory(Directory.GetCurrentDirectory())
  ///     .WithName(new DockerImage("localhost/testcontainers", Guid.NewGuid().ToString("D"), string.Empty))
  ///     .Build();
  ///   </code>
  /// </example>
  [PublicAPI]
  public class ImageFromDockerfileBuilder : AbstractBuilder<ImageFromDockerfileBuilder, IFutureDockerImage, ImageBuildParameters, IImageFromDockerfileConfiguration>, IImageFromDockerfileBuilder<ImageFromDockerfileBuilder>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileBuilder" /> class.
    /// </summary>
    public ImageFromDockerfileBuilder()
      : this(new ImageFromDockerfileConfiguration())
    {
      DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private ImageFromDockerfileBuilder(IImageFromDockerfileConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
      DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override IImageFromDockerfileConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithName(string name)
    {
      return WithName(new DockerImage(name));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithName(IImage name)
    {
      return Merge(DockerResourceConfiguration, new ImageFromDockerfileConfiguration(image: name));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithDockerfile(string dockerfile)
    {
      return Merge(DockerResourceConfiguration, new ImageFromDockerfileConfiguration(dockerfile: dockerfile));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory)
    {
      return Merge(DockerResourceConfiguration, new ImageFromDockerfileConfiguration(dockerfileDirectory: dockerfileDirectory));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithDockerfileDirectory(CommonDirectoryPath commonDirectoryPath, string dockerfileDirectory)
    {
      var dockerfileDirectoryPath = Path.Combine(commonDirectoryPath.DirectoryPath, dockerfileDirectory);
      return Merge(DockerResourceConfiguration, new ImageFromDockerfileConfiguration(dockerfileDirectory: dockerfileDirectoryPath));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithImageBuildPolicy(Func<ImageInspectResponse, bool> imageBuildPolicy)
    {
      return Merge(DockerResourceConfiguration, new ImageFromDockerfileConfiguration(imageBuildPolicy: imageBuildPolicy));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists)
    {
      return Merge(DockerResourceConfiguration, new ImageFromDockerfileConfiguration(deleteIfExists: deleteIfExists));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithBuildArgument(string name, string value)
    {
      var buildArguments = new Dictionary<string, string> { { name, value } };
      return Merge(DockerResourceConfiguration, new ImageFromDockerfileConfiguration(buildArguments: buildArguments));
    }

    /// <inheritdoc />
    public override IFutureDockerImage Build()
    {
      Validate();
      return new FutureDockerImage(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected sealed override ImageFromDockerfileBuilder Init()
    {
      return base.Init().WithImageBuildPolicy(PullPolicy.Always).WithDockerfile("Dockerfile").WithDockerfileDirectory(Directory.GetCurrentDirectory()).WithName(new DockerImage("localhost/testcontainers", Guid.NewGuid().ToString("D"), string.Empty));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
      base.Validate();

      const string reuseNotSupported = "Building an image does not support the reuse feature. To keep the built image, disable the cleanup.";
      _ = Guard.Argument(DockerResourceConfiguration, nameof(IImageFromDockerfileConfiguration.Reuse))
        .ThrowIf(argument => argument.Value.Reuse.HasValue && argument.Value.Reuse.Value, argument => new ArgumentException(reuseNotSupported, argument.Name));
    }

    /// <inheritdoc />
    protected override ImageFromDockerfileBuilder Clone(IResourceConfiguration<ImageBuildParameters> resourceConfiguration)
    {
      return Merge(DockerResourceConfiguration, new ImageFromDockerfileConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ImageFromDockerfileBuilder Merge(IImageFromDockerfileConfiguration oldValue, IImageFromDockerfileConfiguration newValue)
    {
      return new ImageFromDockerfileBuilder(new ImageFromDockerfileConfiguration(oldValue, newValue));
    }
  }
}
