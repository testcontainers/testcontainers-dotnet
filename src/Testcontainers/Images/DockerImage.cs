namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Globalization;
  using System.Text.RegularExpressions;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IImage" />
  [PublicAPI]
  public sealed class DockerImage : IImage
  {
    private const string LatestTag = "latest";

    private const string NightlyTag = "nightly";

    private static readonly char[] TrimChars = [' ', ':', '/'];

    private static readonly char[] SlashChar = ['/'];

    private static readonly Func<string, IImage> GetDockerImage = MatchImage.Match;

    [NotNull]
    private readonly string _repository;

    [CanBeNull]
    private readonly string _registry;

    [CanBeNull]
    private readonly string _tag;

    [CanBeNull]
    private readonly string _digest;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    public DockerImage(IImage image)
      : this(image.Repository, image.Registry, image.Tag, image.Digest)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <example><c>fedora/httpd:version1.0</c> where <c>fedora/httpd</c> is the repository and <c>version1.0</c> the tag.</example>
    public DockerImage(string image)
      : this(GetDockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="registry">The registry.</param>
    /// <param name="tag">The tag.</param>
    /// <param name="digest">The digest.</param>
    /// <param name="hubImageNamePrefix">The Docker Hub image name prefix.</param>
    /// <example><c>fedora/httpd:version1.0</c> where <c>fedora/httpd</c> is the repository and <c>version1.0</c> the tag.</example>
    public DockerImage(
      string repository,
      string registry = null,
      string tag = null,
      string digest = null,
      string hubImageNamePrefix = null)
      : this(
        TrimOrDefault(repository),
        TrimOrDefault(registry),
        TrimOrDefault(tag, tag == null && digest == null ? LatestTag : null),
        TrimOrDefault(digest),
        hubImageNamePrefix == null ? [] : hubImageNamePrefix.Trim(TrimChars).Split(SlashChar, 2, StringSplitOptions.RemoveEmptyEntries))
    {
    }

    private DockerImage(
      string repository,
      string registry,
      string tag,
      string digest,
      string[] substitutions)
    {
      _ = Guard.Argument(repository, nameof(repository))
        .NotNull()
        .NotEmpty()
        .NotUppercase();

      _ = Guard.Argument(substitutions, nameof(substitutions))
        .NotNull();

      // The Docker Hub image name prefix may include namespaces, which we need to extract
      // and prepend to the repository name. The registry itself contains only the hostname.
      switch (substitutions.Length)
      {
        case 2:
          _repository = string.Join("/", substitutions[1], repository);
          _registry = substitutions[0];
          break;
        case 1:
          _repository = repository;
          _registry = substitutions[0];
          break;
        default:
          _repository = repository;
          _registry = registry;
          break;
      }

      _tag = tag;
      _digest = digest;
    }

    /// <inheritdoc />
    public string Repository => _repository;

    /// <inheritdoc />
    public string Registry => _registry;

    /// <inheritdoc />
    public string Tag => _tag;

    /// <inheritdoc />
    public string Digest => _digest;

    /// <inheritdoc />
    public string FullName
    {
      get
      {
        var registry = string.IsNullOrEmpty(Registry) ? string.Empty : $"{Registry}/";
        var tag = string.IsNullOrEmpty(Tag) ? string.Empty : $":{Tag}";
        var digest = string.IsNullOrEmpty(Digest) ? string.Empty : $"@{Digest}";
        return $"{registry}{Repository}{tag}{digest}";
      }
    }

    /// <inheritdoc />
    public string GetHostname()
    {
      return Registry;
    }

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
      if (Tag == null)
      {
        return false;
      }

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

    private static string TrimOrDefault(string value, string defaultValue = null)
    {
      return string.IsNullOrEmpty(value) ? defaultValue : value.Trim(TrimChars);
    }
  }
}
