namespace DotNet.Testcontainers.Internals.Parsers
{
  using System.Linq;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Images;

  internal class MatchImage
  {
    protected const string Part = @"([\w][\w.-]{0,127})";

    private readonly Regex pattern;

    public MatchImage() : this(Part)
    {
    }

    protected MatchImage(string pattern)
    {
      this.pattern = new Regex(pattern, RegexOptions.Compiled);
    }

    public IDockerImage Match(string input)
    {
      var match = this.pattern.Match(input);
      return match.Success ? this.Match(match.Groups.Skip(1).Select(group => group.Value).ToArray()) : null;
    }

    protected virtual IDockerImage Match(params string[] matches)
    {
      return new DockerImage(string.Empty, matches[0], string.Empty);
    }
  }
}
