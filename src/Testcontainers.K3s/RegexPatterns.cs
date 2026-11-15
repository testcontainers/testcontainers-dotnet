namespace Testcontainers.K3s;

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
    /// The pattern that matches the server address of the Kubeconfig cluster entry.
    /// </summary>
    private const string KubeconfigServerPattern = "server:\\s?[:/\\.\\d\\w]+";

#if NET7_0_OR_GREATER
    [GeneratedRegex(KubeconfigServerPattern)]
    public static partial Regex KubeconfigServer();
#else
    // A static class initializes all static fields at once. Lazy<T> creates each
    // regular expression on its first use.
    private static readonly Lazy<Regex> KubeconfigServerRegex = new Lazy<Regex>(() => new Regex(KubeconfigServerPattern));

    public static Regex KubeconfigServer() => KubeconfigServerRegex.Value;
#endif
}
