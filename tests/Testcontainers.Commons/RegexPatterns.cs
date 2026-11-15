namespace DotNet.Testcontainers.Commons;

/// <summary>
/// Provides the regular expressions that use a constant pattern.
/// </summary>
internal static partial class RegexPatterns
{
    /// <summary>
    /// The pattern that matches a Dockerfile <c>FROM</c> instruction.
    /// </summary>
    private const string DockerfileFromLinePattern = "^FROM\\s+(?<arg>--\\S+\\s)*(?<image>\\S+).*";

    [GeneratedRegex(DockerfileFromLinePattern, RegexOptions.IgnoreCase)]
    public static partial Regex DockerfileFromLine();
}
