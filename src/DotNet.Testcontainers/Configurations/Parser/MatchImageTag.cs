namespace DotNet.Testcontainers.Configuration.Parser
{
  using DotNet.Testcontainers.Images;

  internal sealed class MatchImageTag : MatchImage
  {
    internal MatchImageTag() : base(@"([\w][\w.-]{0,127})\:([\w][\w.-]{0,127})") // bar:1.0.0
    {
    }

    protected override GenericImage Match(params string[] matches)
    {
      return new GenericImage(string.Empty, matches[0], matches[1]);
    }
  }
}
