namespace DotNet.Testcontainers.Core.Parser
{
  using DotNet.Testcontainers.Core.Image;

  internal sealed class MatchImageComplete : MatchImage
  {
    internal MatchImageComplete() : base(@"([\w][\w.-]{0,127})\/([\w][\w.-]{0,127})\:([\w][\w.-]{0,127})") // Matches foo/bar:1.0.0
    {
    }

    protected override TestcontainersImage Match(params string[] matches)
    {
      return new TestcontainersImage(matches[0], matches[1], matches[2]);
    }
  }
}
