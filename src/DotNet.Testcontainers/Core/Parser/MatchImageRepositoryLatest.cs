namespace DotNet.Testcontainers.Core.Parser
{
  using DotNet.Testcontainers.Core.Images;

  internal sealed class MatchImageRepositoryLatest : MatchImage
  {
    public MatchImageRepositoryLatest() : base($@"{Part}\/{Part}") // Matches foo/bar
    {
    }

    protected override TestcontainersImage Match(params string[] matches)
    {
      return new TestcontainersImage(matches[0], matches[1], string.Empty);
    }
  }
}
