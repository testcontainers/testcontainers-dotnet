namespace DotNet.Testcontainers.Configuration.Parser
{
  using DotNet.Testcontainers.Images;

  internal sealed class MatchImageComplete : MatchImage
  {
    internal MatchImageComplete() : base(@"([\w][\w.-]{0,127})\/([\w][\w.-]{0,127})\:([\w][\w.-]{0,127})") // Matches foo/bar:1.0.0
    {
    }

    protected override GenericImage Match(params string[] matches)
    {
      return new GenericImage(matches[0], matches[1], matches[2]);
    }
  }
}
