namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Globalization;
  using System.Linq;
  using System.Text.RegularExpressions;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IImage" />
  [PublicAPI]
  public sealed class DockerImage : IImage
  {
    private const string LatestTag = "latest";

    private const string NightlyTag = "nightly";

    private static readonly Func<string, IImage> GetDockerImage = MatchImage.Match;

    private static readonly char[] TrimChars = { ' ', ':', '/' };

    private static readonly char[] HostnameIdentifierChars = { '.', ':' };

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

        return firstSegmentOfRepository.IndexOfAny(HostnameIdentifierChars) >= 0 ? firstSegmentOfRepository : null;
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

    /// <inheritdoc />
    public bool MatchLatestOrNightly()
    {
      return MatchVersion((string tag) => LatestTag.Equals(tag) || NightlyTag.Equals(tag));
    }

    /// <inheritdoc />
    public bool MatchVersion(Predicate<string> predicate)
    {
      return predicate(Tag);
    }

    /// <inheritdoc />
    public bool MatchVersion(Predicate<Version> predicate)
    {
      var versionMatch = Regex.Match(Tag, "^(\\d+)(\\.\\d+)?(\\.\\d+)?", RegexOptions.None, TimeSpan.FromSeconds(1));

      if (!versionMatch.Success)
      {
        return false;
      }

      if (Version.TryParse(versionMatch.Value, out var version))
      {
        return predicate(version);
      }

      // If the Regex matches and Version.TryParse(string?, out Version?) fails then it means it is a major version only (i.e. without any dot separator)
      return predicate(new Version(int.Parse(versionMatch.Groups[1].Value, NumberStyles.None), 0));
    }

    private static string TrimOrDefault(string value, string defaultValue = default)
    {
      return string.IsNullOrEmpty(value) ? defaultValue : value.Trim(TrimChars);
    }
  }
}
