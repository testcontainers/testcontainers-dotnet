namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Linq;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IImage" />
  [PublicAPI]
  public sealed class DockerImage : IImage
  {
    private const string LatestTag = "latest";

    private static readonly Func<string, IImage> GetDockerImage = MatchImage.Match;

    private static readonly char[] TrimChars = { ' ', ':', '/' };

    private readonly string _hubImageNamePrefix;

    private readonly Lazy<string> _lazyFullName;

    private readonly Lazy<string> _lazyHostname;

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
      string tag = null,
      string hubImageNamePrefix = null)
    {
      _ = Guard.Argument(repository, nameof(repository))
        .NotNull()
        .NotUppercase();

      _ = Guard.Argument(name, nameof(name))
        .NotNull()
        .NotEmpty()
        .NotUppercase();

      _hubImageNamePrefix = TrimOrDefault(hubImageNamePrefix);

      Repository = TrimOrDefault(repository, repository);
      Name = TrimOrDefault(name, name);
      Tag = TrimOrDefault(tag, LatestTag);

      _lazyFullName = new Lazy<string>(() =>
      {
        var imageComponents = new[] { _hubImageNamePrefix, Repository, Name }
          .Where(imageComponent => !string.IsNullOrEmpty(imageComponent));

        return string.Join("/", imageComponents) + ":" + Tag;
      });

      _lazyHostname = new Lazy<string>(() =>
      {
        var firstSegmentOfRepository = new[] { _hubImageNamePrefix, Repository }
          .Where(imageComponent => !string.IsNullOrEmpty(imageComponent))
          .DefaultIfEmpty(string.Empty)
          .First()
          .Split('/')[0];

        if (firstSegmentOfRepository.IndexOfAny(new[] { '.', ':' }) >= 0)
        {
          return firstSegmentOfRepository;
        }
        else
        {
          return null;
        }
      });
    }

    /// <inheritdoc />
    public string Repository { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Tag { get; }

    /// <inheritdoc />
    public string FullName => _lazyFullName.Value;

    /// <inheritdoc />
    public string GetHostname() => _lazyHostname.Value;

    private static string TrimOrDefault(string value, string defaultValue = default)
    {
      return string.IsNullOrEmpty(value) ? defaultValue : value.Trim(TrimChars);
    }
  }
}
