using System.Text.RegularExpressions;

namespace Testcontainers.Gitlab.RegexPatterns;

/// <summary>
/// This class contains regex patterns that are used in gitlab.
/// </summary>
public static partial class GitlabRegex
{
    /// <summary>
    /// GitLab Personal Access Token
    /// </summary>
    public static Regex GitlabPersonalAccessToken => new(@"glpat-[0-9a-zA-Z_\-]{20}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    /// <summary>
    /// Regex Pattern to find the gitlab root password
    /// </summary>
    public static Regex GitlabRootPassword => new(@"Password: .*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
}