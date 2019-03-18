namespace DotNet.Testcontainers.Core.Parser
{
  using DotNet.Testcontainers.Core.Images;

  internal sealed class MatchImageComplete : MatchImage
  {
    public MatchImageComplete() : base($"{Word}\\/{Word}\\:{Word}") // Matches foo/bar:1.0.0
    {
    }

    protected override TestcontainersImage Match(params string[] matches)
    {
      return new TestcontainersImage(matches[0], matches[1], matches[2]);
    }
  }
}
