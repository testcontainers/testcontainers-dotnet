namespace DotNet.Testcontainers.Core.Parser
{
  using DotNet.Testcontainers.Core.Images;

  internal sealed class MatchImageTag : MatchImage
  {
    public MatchImageTag() : base($"{Word}\\:{Word}") // bar:1.0.0
    {
    }

    protected override TestcontainersImage Match(params string[] matches)
    {
      return new TestcontainersImage(string.Empty, matches[0], matches[1]);
    }
  }
}
