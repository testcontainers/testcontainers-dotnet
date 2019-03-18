namespace DotNet.Testcontainers.Core.Parser
{
  using System.Linq;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Core.Images;

  internal class MatchImage
  {
    public const string Word = "([\\w][\\w.-]{0,127})";

    public static readonly MatchImage Complete = new MatchImageComplete();

    public static readonly MatchImage Registry = new MatchImageRegistry();

    public static readonly MatchImage Latest = new MatchImageLatest();

    public static readonly MatchImage Tag = new MatchImageTag();

    public static readonly MatchImage Any = new MatchImage();

    public static readonly MatchImage[] Matcher = { Registry, Complete, Latest, Tag, Any };

    private readonly Regex pattern;

    protected MatchImage(string pattern) : this(new Regex(pattern, RegexOptions.Compiled))
    {
    }

    protected MatchImage(Regex pattern)
    {
      this.pattern = pattern;
    }

    private MatchImage() : this(Word)
    {
    }

    public TestcontainersImage Match(string input)
    {
      // Maybe we can use a better functional approache here?
      var match = this.pattern.Match(input);

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
