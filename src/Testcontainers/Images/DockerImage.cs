namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Linq;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerImage" />
  [PublicAPI]
  public sealed class DockerImage : IDockerImage
  {
    private static readonly Func<string, IDockerImage> GetDockerImage = MatchImage.Match;

    private readonly string hubImageNamePrefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    [PublicAPI]
    public DockerImage(IDockerImage image)
      : this(image.Repository, image.Name, image.Tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>"fedora/httpd:version1.0" where "fedora" is the repository, "httpd" the name and "version1.0" the tag.</example>
    [PublicAPI]
    public DockerImage(string image)
      : this(GetDockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="repository">The Docker image repository.</param>
    /// <param name="name">The Docker image name.</param>
    /// <param name="tag">The Docker image tag.</param>
    /// <param name="hubImageNamePrefix">The Docker Hub image name prefix.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>"fedora/httpd:version1.0" where "fedora" is the repository, "httpd" the name and "version1.0" the tag.</example>
    [PublicAPI]
    public DockerImage(
      string repository,
      string name,
      string tag,
      string hubImageNamePrefix = null)
    {
      Guard.Argument(repository, nameof(repository))
        .NotNull()
        .NotUppercase();

      Guard.Argument(name, nameof(name))
        .NotNull()
        .NotEmpty()
        .NotUppercase();

      Guard.Argument(tag, nameof(tag))
        .NotNull();

      this.hubImageNamePrefix = hubImageNamePrefix;

      this.Repository = repository;
      this.Name = name;
      this.Tag = tag;

      if (this.Tag.Length == 0)
      {
        this.Tag = "latest";
      }
    }

    /// <inheritdoc />
    public string Repository { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Tag { get; }

    /// <inheritdoc />
    public string FullName
    {
      get
      {
        var dockerImageParts = new[] { this.hubImageNamePrefix, this.Repository, this.Name }
          .Where(dockerImagePart => !string.IsNullOrEmpty(dockerImagePart))
          .Select(dockerImagePart => dockerImagePart.Trim('/', ':'))
          .Where(dockerImagePart => !string.IsNullOrEmpty(dockerImagePart));
        return string.Join("/", dockerImageParts) + ":" + this.Tag;
      }
    }

    /// <inheritdoc />
    public string GetHostname()
    {
      var firstSegmentOfRepository = (string.IsNullOrEmpty(this.hubImageNamePrefix) ? this.Repository : this.hubImageNamePrefix).Split('/').First();
      return firstSegmentOfRepository.IndexOfAny(new[] { '.', ':' }) >= 0 ? firstSegmentOfRepository : null;
    }
  }
}
