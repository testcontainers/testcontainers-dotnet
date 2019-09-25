namespace DotNet.Testcontainers.Core.Parser
{
  using System.Linq;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Core.Images;

  internal class MatchImage
  {
    public const string Part = @"([\w][\w.-]{0,127})";

    public static readonly MatchImage[] Matcher =
    {
      new MatchImageRegistry(),
      new MatchImageRepositoryTag(),
      new MatchImageRepositoryLatest(),
      new MatchImageTag(),
      new MatchImage(),
    };

    protected MatchImage(Regex pattern)
    {
      this.Pattern = pattern;
    }

    protected MatchImage(string pattern) : this(new Regex(pattern, RegexOptions.Compiled))
    {
    }

    private MatchImage() : this(Part)
    {
    }

    private Regex Pattern { get; }

    public TestcontainersImage Match(string input)
    {
      // Maybe we can use a better functional approach here?
      var match = this.Pattern.Match(input);

      if (match.Success)
      {
        return this.Match(match.Groups.Cast<Group>().Skip(1).Select(group => group.Value).ToArray());
      }
      else
      {
        return null;
      }
    }

    protected virtual TestcontainersImage Match(params string[] matches)
    {
      return new TestcontainersImage(string.Empty, matches[0], string.Empty);
    }
  }
}
