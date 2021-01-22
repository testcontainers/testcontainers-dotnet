namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IImageFromDockerfileBuilder" />
  [PublicAPI]
  public sealed class ImageFromDockerfileBuilder : IImageFromDockerfileBuilder
  {
    private readonly IImageFromDockerfileConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileBuilder" /> class.
    /// </summary>
    public ImageFromDockerfileBuilder()
      : this(
        Apply(
          image: new DockerImage($"testcontainers:{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"),
          dockerfile: "Dockerfile",
          dockerfileDirectory: ".",
          labels: DefaultLabels.Instance))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileBuilder" /> class.
    /// </summary>
    /// <param name="configuration">The Dockerfile configuration.</param>
    private ImageFromDockerfileBuilder(IImageFromDockerfileConfiguration configuration)
    {
      this.configuration = configuration;
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithName(string name)
    {
      return Build(this, Apply(image: new DockerImage(name)));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithName(IDockerImage name)
    {
      return Build(this, Apply(image: name));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithDockerfile(string dockerfile)
    {
      return Build(this, Apply(dockerfile: dockerfile));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory)
    {
      return Build(this, Apply(dockerfileDirectory: dockerfileDirectory));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists)
    {
      return Build(this, Apply(deleteIfExists: deleteIfExists));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithLabel(string name, string value)
    {
      var labels = new Dictionary<string, string> { { name, value } };
      return Build(this, Apply(labels: labels));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithResourceReaperSessionId(Guid resourceReaperSessionId)
    {
      return this.WithLabel(ResourceReaper.ResourceReaperSessionLabel, resourceReaperSessionId.ToString("D"));
    }

    /// <inheritdoc />
    public Task<string> Build()
    {
      var client = new TestcontainersClient();
      return client.BuildAsync(this.configuration);
    }

    private static IImageFromDockerfileConfiguration Apply(
      IDockerImage image = null,
      string dockerfile = null,
      string dockerfileDirectory = null,
      bool deleteIfExists = true,
      IReadOnlyDictionary<string, string> labels = null)
    {
      return new ImageFromDockerfileConfiguration(image, dockerfile, dockerfileDirectory, deleteIfExists, labels);
    }

    private static IImageFromDockerfileBuilder Build(
      ImageFromDockerfileBuilder previous,
      IImageFromDockerfileConfiguration next)
    {
      var deleteIfExists = next.DeleteIfExists && previous.configuration.DeleteIfExists;
      var labels = BuildConfiguration.Combine(next.Labels, previous.configuration.Labels);

      var image = new[] { next.Image, previous.configuration.Image }.First(config => config != null);
      var dockerfile = new[] { next.Dockerfile, previous.configuration.Dockerfile }.First(config => config != null);
      var dockerfileDirectory = new[] { next.DockerfileDirectory, previous.configuration.DockerfileDirectory }.First(config => config != null);

      var mergedConfiguration = Apply(
        image,
        dockerfile,
        dockerfileDirectory,
        deleteIfExists,
        labels);

      return new ImageFromDockerfileBuilder(mergedConfiguration);
    }
  }
}
