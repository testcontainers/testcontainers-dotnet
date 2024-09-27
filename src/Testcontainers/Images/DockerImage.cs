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

    private static readonly Func<string, IImage> GetDockerImage = MatchImage.Match;

    [NotNull]
    private readonly string _repository;

    [CanBeNull]
    private readonly string _registry;

    [CanBeNull]
    private readonly string _tag;

    [CanBeNull]
    private readonly string _digit;

    [CanBeNull]
    private readonly string _hubImageNamePrefix;

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
    /// <param name="name">The name.</param>
    [Obsolete("We will remove this construct and replace it with a more efficient implementation. Please use 'DockerImage(string, string = null, string = null, string = null, string = null)' instead. All arguments except for 'repository' (the first) are optional.")]
    public DockerImage(string repository, string name)
      : this(string.Join("/", repository, name).Trim('/'))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="name">The name.</param>
    /// <param name="tag">The tag.</param>
    [Obsolete("We will remove this construct and replace it with a more efficient implementation. Please use 'DockerImage(string, string = null, string = null, string = null, string = null)' instead. All arguments except for 'repository' (the first) are optional.")]
    public DockerImage(string repository, string name, string tag)
      : this(string.Join("/", repository, name).Trim('/') + (":" + tag).TrimEnd(':'))
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
    {
      _ = Guard.Argument(repository, nameof(repository))
        .NotNull()
        .NotEmpty()
        .NotUppercase();

      var defaultTag = tag == null && digest == null ? LatestTag : null;

      _repository = TrimOrDefault(repository);
      _registry = TrimOrDefault(registry);
      _tag = TrimOrDefault(tag, defaultTag);
      _digit = TrimOrDefault(digest);
      _hubImageNamePrefix = TrimOrDefault(hubImageNamePrefix);
    }

    /// <inheritdoc />
    public string Repository => _repository;

    /// <inheritdoc />
    public string Registry => string.IsNullOrEmpty(_hubImageNamePrefix) ? _registry : _hubImageNamePrefix;

    /// <inheritdoc />
    public string Tag => _tag;

    /// <inheritdoc />
    public string Digest => _digit;

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
    [Obsolete("We will remove this property, it does not follow the DSL. Use the 'Repository' property instead.")]
    public string Name => GetBackwardsCompatibleName();

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

    private string GetBackwardsCompatibleName()
    {
      // The last index will never be a `/`, we trim it in the constructor.
      var lastIndex = _repository.LastIndexOf('/');
      return lastIndex == -1 ? _repository : _repository.Substring(lastIndex + 1);
    }

    private static string TrimOrDefault(string value, string defaultValue = null)
    {
      return string.IsNullOrEmpty(value) ? defaultValue : value.Trim(TrimChars);
    }
  }
}
