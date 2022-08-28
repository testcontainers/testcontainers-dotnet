namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IImageFromDockerfileBuilder" />
  [PublicAPI]
  public class ImageFromDockerfileBuilder : AbstractBuilder<IImageFromDockerfileBuilder, IImageFromDockerfileConfiguration>, IImageFromDockerfileBuilder
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileBuilder" /> class.
    /// </summary>
    public ImageFromDockerfileBuilder()
      : this(new ImageFromDockerfileConfiguration(
        dockerEndpointAuthenticationConfiguration: TestcontainersSettings.OS.DockerEndpointAuthConfig,
        image: new DockerImage($"testcontainers:{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"),
        dockerfile: "Dockerfile",
        dockerfileDirectory: ".",
        labels: DefaultLabels.Instance,
        buildArguments: new Dictionary<string, string>()))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker image configuration.</param>
    private ImageFromDockerfileBuilder(IImageFromDockerfileConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithName(string name)
    {
      return this.MergeNewConfiguration(new ImageFromDockerfileConfiguration(image: new DockerImage(name)));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithName(IDockerImage name)
    {
      return this.MergeNewConfiguration(new ImageFromDockerfileConfiguration(image: name));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithDockerfile(string dockerfile)
    {
      return this.MergeNewConfiguration(new ImageFromDockerfileConfiguration(dockerfile: dockerfile));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory)
    {
      return this.MergeNewConfiguration(new ImageFromDockerfileConfiguration(dockerfileDirectory: dockerfileDirectory));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists)
    {
      return this.MergeNewConfiguration(new ImageFromDockerfileConfiguration(deleteIfExists: deleteIfExists));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithBuildArgument(string name, string value)
    {
      var buildArgs = new Dictionary<string, string> { { name, value } };
      return this.MergeNewConfiguration(new ImageFromDockerfileConfiguration(buildArguments: buildArgs));
    }

    /// <inheritdoc />
    public Task<string> Build()
    {
      _ = Guard.Argument(this.DockerResourceConfiguration.DockerEndpointAuthConfig, nameof(IDockerResourceConfiguration.DockerEndpointAuthConfig))
        .DockerEndpointAuthConfigIsSet();

      ITestcontainersClient client = new TestcontainersClient(this.DockerResourceConfiguration.SessionId, this.DockerResourceConfiguration.DockerEndpointAuthConfig, TestcontainersSettings.Logger);
      return client.BuildAsync(this.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override IImageFromDockerfileBuilder MergeNewConfiguration(IDockerResourceConfiguration dockerResourceConfiguration)
    {
      return this.MergeNewConfiguration(new ImageFromDockerfileConfiguration(dockerResourceConfiguration));
    }

    /// <summary>
    /// Merges the current with the new Docker resource configuration.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The new Docker resource configuration.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    protected virtual IImageFromDockerfileBuilder MergeNewConfiguration(IImageFromDockerfileConfiguration dockerResourceConfiguration)
    {
      var dockerEndpointAuthConfig = BuildConfiguration.Combine(dockerResourceConfiguration.DockerEndpointAuthConfig, this.DockerResourceConfiguration.DockerEndpointAuthConfig);
      var image = BuildConfiguration.Combine(dockerResourceConfiguration.Image, this.DockerResourceConfiguration.Image);
      var dockerfile = BuildConfiguration.Combine(dockerResourceConfiguration.Dockerfile, this.DockerResourceConfiguration.Dockerfile);
      var dockerfileDirectory = BuildConfiguration.Combine(dockerResourceConfiguration.DockerfileDirectory, this.DockerResourceConfiguration.DockerfileDirectory);
      var deleteIfExists = dockerResourceConfiguration.DeleteIfExists && this.DockerResourceConfiguration.DeleteIfExists;
      var labels = BuildConfiguration.Combine(dockerResourceConfiguration.Labels, this.DockerResourceConfiguration.Labels);
      var buildArgs = BuildConfiguration.Combine(dockerResourceConfiguration.BuildArguments, this.DockerResourceConfiguration.BuildArguments);
      return new ImageFromDockerfileBuilder(new ImageFromDockerfileConfiguration(dockerEndpointAuthConfig, image, dockerfile, dockerfileDirectory, deleteIfExists, labels, buildArgs));
    }
  }
}
