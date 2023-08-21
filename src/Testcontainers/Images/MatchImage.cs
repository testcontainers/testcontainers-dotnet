namespace DotNet.Testcontainers.Images
{
  using System.Text.RegularExpressions;

  internal static class MatchImage
  {
    private static readonly Regex _imagePattern = new Regex(@"^((?<registry>[^\.\/\:]+(\.[^\.\/\:]*)+(\:[^\/]+)?|[^\:\/]+(\:[^\/]+)|localhost)\/)?(?<repository>[^\:\n]*)(\:(?<tag>.+)?)?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    public static IImage Match(string image)
    {
      _ = Guard.Argument(image, nameof(image))
        .NotNull()
        .NotEmpty();

      var match = _imagePattern.Match(image);

      var registry = match.Groups[1].Value;
      var repository = match.Groups[2].Value;
      var tag = match.Groups[3].Value;

      return new DockerImage(registry, repository, tag);
    }
  }
}
