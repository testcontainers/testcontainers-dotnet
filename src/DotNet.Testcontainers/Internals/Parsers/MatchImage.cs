namespace DotNet.Testcontainers.Internals.Parsers
{
  using System.Linq;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Images;

  internal class MatchImage
  {
    protected const string Part = @"([\w][\w.-]{0,127})";

    public MatchImage() : this(Part)
    {
    }

    protected MatchImage(string pattern)
    {
      this.Pattern = new Regex(pattern, RegexOptions.Compiled);
    }

    private Regex Pattern { get; }

    public IDockerImage Match(string input)
    {
      var match = this.Pattern.Match(input);
      return match.Success ? this.Match(match.Groups.Cast<Group>().Skip(1).Select(group => group.Value).ToArray()) : null;
    }

    protected virtual IDockerImage Match(params string[] matches)
    {
      return new DockerImage(string.Empty, matches[0], string.Empty);
    }
  }
}
