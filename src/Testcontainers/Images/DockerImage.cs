namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Linq;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IImage" />
  [PublicAPI]
  public sealed class DockerImage : IImage
  {
    private static readonly Func<string, IImage> GetDockerImage = MatchImage.Match;

    private readonly string _hubImageNamePrefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    public DockerImage(IImage image)
      : this(image.Registry, image.Repository, image.Tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>"fedora/httpd:version1.0" where "fedora" is the repository, "httpd" the name and "version1.0" the tag.</example>
    public DockerImage(string image)
      : this(GetDockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="registry">The registry.</param>
    /// <param name="repository">The repository.</param>
    /// <param name="tag">The tag.</param>
    /// <param name="hubImageNamePrefix">The Docker Hub image name prefix.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>"docker.io/fedora/httpd:version1.0" where "docker.io" is the registry, "fedora/httpd" is the repository and "version1.0" the tag.</example>
    public DockerImage(
      string registry,
      string repository,
      string tag,
      string hubImageNamePrefix = null)
    {
      _ = Guard.Argument(registry, nameof(registry))
        .NotNull()
        .NotUppercase();

      _ = Guard.Argument(repository, nameof(repository))
        .NotNull()
        .NotEmpty()
        .NotUppercase();

      _hubImageNamePrefix = hubImageNamePrefix;

      Registry = registry;
      Repository = repository;
      Tag = string.IsNullOrEmpty(tag) ? "latest" : tag;
    }

    /// <inheritdoc />
    public string Registry { get; }

    /// <inheritdoc />
    public string Repository { get; }

    /// <inheritdoc />
    public string Tag { get; }

    /// <inheritdoc />
    public string FullName
    {
      get
      {
        var imageComponents = new[] { _hubImageNamePrefix, Registry, Repository }
          .Where(imageComponent => !string.IsNullOrEmpty(imageComponent))
          .Select(imageComponent => imageComponent.Trim('/', ':'))
          .Where(imageComponent => !string.IsNullOrEmpty(imageComponent));
        return string.Join("/", imageComponents) + ":" + Tag;
      }
    }

    /// <inheritdoc />
    public string GetHostname()
    {
      var firstSegmentOfRegistry = (string.IsNullOrEmpty(_hubImageNamePrefix) ? Registry : _hubImageNamePrefix).Split('/')[0];
      return firstSegmentOfRegistry.IndexOfAny(new[] { '.', ':' }) >= 0 ? firstSegmentOfRegistry : null;
    }
  }
}
