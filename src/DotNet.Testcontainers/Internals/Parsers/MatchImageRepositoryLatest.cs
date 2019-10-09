namespace DotNet.Testcontainers.Internals.Parsers
{
  using DotNet.Testcontainers.Images;

  internal sealed class MatchImageRepositoryLatest : MatchImage
  {
    public MatchImageRepositoryLatest() : base($@"{Part}\/{Part}") // Matches foo/bar
    {
    }

    protected override IDockerImage Match(params string[] matches)
    {
      return new DockerImage(matches[0], matches[1], string.Empty);
    }
  }
}
