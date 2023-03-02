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

    private readonly string hubImageNamePrefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    public DockerImage(IImage image)
      : this(image.Repository, image.Name, image.Tag)
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
    /// <param name="repository">The repository.</param>
    /// <param name="name">The name.</param>
    /// <param name="tag">The tag.</param>
    /// <param name="hubImageNamePrefix">The Docker Hub image name prefix.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>"fedora/httpd:version1.0" where "fedora" is the repository, "httpd" the name and "version1.0" the tag.</example>
    public DockerImage(
      string repository,
      string name,
      string tag,
      string hubImageNamePrefix = null)
    {
      _ = Guard.Argument(repository, nameof(repository))
        .NotNull()
        .NotUppercase();

      _ = Guard.Argument(name, nameof(name))
        .NotNull()
        .NotEmpty()
        .NotUppercase();

      this.hubImageNamePrefix = hubImageNamePrefix;

      this.Repository = repository;
      this.Name = name;
      this.Tag = string.IsNullOrEmpty(tag) ? "latest" : tag;
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
        var imageComponents = new[] { this.hubImageNamePrefix, this.Repository, this.Name }
          .Where(imageComponent => !string.IsNullOrEmpty(imageComponent))
          .Select(imageComponent => imageComponent.Trim('/', ':'))
          .Where(imageComponent => !string.IsNullOrEmpty(imageComponent));
        return string.Join("/", imageComponents) + ":" + this.Tag;
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
