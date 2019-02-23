namespace DotNet.Testcontainers.Core.Parser
{
  using DotNet.Testcontainers.Core.Image;

  internal sealed class MatchImageRepository : MatchImage
  {
    internal MatchImageRepository() : base(@"([\w][\w.-]{0,127})\/([\w][\w.-]{0,127})") // Matches foo/bar
    {
    }

    protected override TestcontainersImage Match(params string[] matches)
    {
      return new TestcontainersImage(matches[0], matches[1], string.Empty);
    }
  }
}
