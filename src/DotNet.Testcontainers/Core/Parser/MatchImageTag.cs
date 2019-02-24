namespace DotNet.Testcontainers.Core.Parser
{
  using DotNet.Testcontainers.Core.Images;

  internal sealed class MatchImageTag : MatchImage
  {
    internal MatchImageTag() : base(@"([\w][\w.-]{0,127})\:([\w][\w.-]{0,127})") // bar:1.0.0
    {
    }

    protected override TestcontainersImage Match(params string[] matches)
    {
      return new TestcontainersImage(string.Empty, matches[0], matches[1]);
    }
  }
}
