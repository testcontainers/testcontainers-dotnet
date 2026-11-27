namespace DotNet.Testcontainers
{
#if !NET7_0_OR_GREATER
  using System;
#endif
  using System.Text.RegularExpressions;

  /// <summary>
  /// Provides the regular expressions that use a constant pattern.
  /// </summary>
  /// <remarks>
  /// .NET 7 and later generate the regular expressions at compile time with
  /// <c>GeneratedRegex</c>. On .NET Standard, which does not support the source
  /// generator, each regular expression is created at runtime on its first use.
  /// </remarks>
  internal static partial class RegexPatterns
  {
    /// <summary>
    /// The pattern that matches a valid build secret or SSH id.
    /// </summary>
    /// <remarks>
    /// The id is part of the path of the file that carries the build secret inside the
    /// Docker CLI container, and part of the Docker CLI argument that references it.
    /// </remarks>
    private const string BuildSecretIdPattern = "^[A-Za-z0-9][A-Za-z0-9_.-]*$";

    /// <summary>
    /// The pattern that matches a valid Docker Compose project name.
    /// </summary>
    private const string ComposeProjectNamePattern = "^[a-z0-9][a-z0-9_-]*$";

    /// <summary>
    /// The pattern that matches the leading current directory segment of a
    /// relative path, e.g., <c>./</c> or <c>.\</c>.
    /// </summary>
    private const string CurrentDirectoryPrefixPattern = "^\\.(\\/|\\\\)";

    /// <summary>
    /// The pattern that matches a Dockerfile <c>ARG</c> instruction that
    /// declares a default value.
    /// </summary>
    private const string DockerfileArgLinePattern = "^ARG\\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)=(?:\"(?<value>[^\"]*)\"|'(?<value>[^']*)'|(?<value>\\S+))";

    /// <summary>
    /// The pattern that matches a Dockerfile <c>FROM</c> instruction.
    /// </summary>
    private const string DockerfileFromLinePattern = "^FROM\\s+(?<arg>--\\S+\\s)*(?<image>\\S+).*";

    /// <summary>
    /// The pattern that matches a Dockerfile variable reference,
    /// e.g., <c>$name</c> or <c>${name}</c>.
    /// </summary>
    private const string DockerfileVariablePattern = "\\$(\\{(?<name>[A-Za-z_][A-Za-z0-9_]*)\\}|(?<name>[A-Za-z_][A-Za-z0-9_]*))";

    /// <summary>
    /// The pattern that matches a Docker image reference.
    /// </summary>
    /// <remarks>
    /// The pattern is taken from the Go implementation:
    /// https://github.com/distribution/reference/blob/8c942b0459dfdcc5b6685581dd0a5a470f615bff/reference.go.
    /// </remarks>
    private const string ImageReferencePattern = "^(?<remote_name>(?:(?:(?:[a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9])(?:\\.(?:[a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9]))*|\\[(?:[a-fA-F0-9:]+)\\])(?::[0-9]+)?\\/)?[a-z0-9]+(?:(?:[._]|__|[-]+)[a-z0-9]+)*(?:\\/[a-z0-9]+(?:(?:[._]|__|[-]+)[a-z0-9]+)*)*)(?::(?<tag>[\\w][\\w.-]{0,127}))?(?:@(?<digest>[A-Za-z][A-Za-z0-9]*(?:[-_+.][A-Za-z][A-Za-z0-9]*)*[:][0-9A-Fa-f]{32,}))?$";

    /// <summary>
    /// The pattern that matches the leading numeric version of a Docker image tag,
    /// e.g., <c>1.2.3</c> of <c>1.2.3-alpine</c>.
    /// </summary>
    private const string ImageTagVersionPattern = "^(\\d+)(\\.\\d+)?(\\.\\d+)?";

    /// <summary>
    /// The pattern that matches a character that is not an ASCII letter or digit.
    /// </summary>
    private const string NonAlphanumericCharacterPattern = "[^a-zA-Z0-9]";

    /// <summary>
    /// The pattern that matches a regular expression metacharacter
    /// (-, [, ], /, {, }, (, ), +, ?, ., \, ^, $, |).
    /// </summary>
    private const string RegexMetacharacterPattern = "[\\-\\[\\]\\/\\{\\}\\(\\)\\+\\?\\.\\\\\\^\\$\\|]";

#if NET7_0_OR_GREATER
    [GeneratedRegex(BuildSecretIdPattern)]
    public static partial Regex BuildSecretId();

    [GeneratedRegex(ComposeProjectNamePattern)]
    public static partial Regex ComposeProjectName();

    [GeneratedRegex(CurrentDirectoryPrefixPattern)]
    public static partial Regex CurrentDirectoryPrefix();

    [GeneratedRegex(DockerfileArgLinePattern, RegexOptions.IgnoreCase)]
    public static partial Regex DockerfileArgLine();

    [GeneratedRegex(DockerfileFromLinePattern, RegexOptions.IgnoreCase)]
    public static partial Regex DockerfileFromLine();

    [GeneratedRegex(DockerfileVariablePattern)]
    public static partial Regex DockerfileVariable();

    [GeneratedRegex(ImageReferencePattern)]
    public static partial Regex ImageReference();

    [GeneratedRegex(ImageTagVersionPattern)]
    public static partial Regex ImageTagVersion();

    [GeneratedRegex(NonAlphanumericCharacterPattern)]
    public static partial Regex NonAlphanumericCharacter();

    [GeneratedRegex(RegexMetacharacterPattern)]
    public static partial Regex RegexMetacharacter();
#else
    // A static class initializes all static fields at once. Lazy<T> creates each
    // regular expression on its first use, which matters for the compiled image
    // reference pattern.
    private static readonly Lazy<Regex> BuildSecretIdRegex = new Lazy<Regex>(() => new Regex(BuildSecretIdPattern));

    private static readonly Lazy<Regex> ComposeProjectNameRegex = new Lazy<Regex>(() => new Regex(ComposeProjectNamePattern));

    private static readonly Lazy<Regex> CurrentDirectoryPrefixRegex = new Lazy<Regex>(() => new Regex(CurrentDirectoryPrefixPattern));

    private static readonly Lazy<Regex> DockerfileArgLineRegex = new Lazy<Regex>(() => new Regex(DockerfileArgLinePattern, RegexOptions.IgnoreCase));

    private static readonly Lazy<Regex> DockerfileFromLineRegex = new Lazy<Regex>(() => new Regex(DockerfileFromLinePattern, RegexOptions.IgnoreCase));

    private static readonly Lazy<Regex> DockerfileVariableRegex = new Lazy<Regex>(() => new Regex(DockerfileVariablePattern));

    private static readonly Lazy<Regex> ImageReferenceRegex = new Lazy<Regex>(() => new Regex(ImageReferencePattern, RegexOptions.Compiled));

    private static readonly Lazy<Regex> ImageTagVersionRegex = new Lazy<Regex>(() => new Regex(ImageTagVersionPattern));

    private static readonly Lazy<Regex> NonAlphanumericCharacterRegex = new Lazy<Regex>(() => new Regex(NonAlphanumericCharacterPattern));

    private static readonly Lazy<Regex> RegexMetacharacterRegex = new Lazy<Regex>(() => new Regex(RegexMetacharacterPattern));

    public static Regex BuildSecretId() => BuildSecretIdRegex.Value;

    public static Regex ComposeProjectName() => ComposeProjectNameRegex.Value;

    public static Regex CurrentDirectoryPrefix() => CurrentDirectoryPrefixRegex.Value;

    public static Regex DockerfileArgLine() => DockerfileArgLineRegex.Value;

    public static Regex DockerfileFromLine() => DockerfileFromLineRegex.Value;

    public static Regex DockerfileVariable() => DockerfileVariableRegex.Value;

    public static Regex ImageReference() => ImageReferenceRegex.Value;

    public static Regex ImageTagVersion() => ImageTagVersionRegex.Value;

    public static Regex NonAlphanumericCharacter() => NonAlphanumericCharacterRegex.Value;

    public static Regex RegexMetacharacter() => RegexMetacharacterRegex.Value;
#endif
  }
}
