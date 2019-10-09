namespace DotNet.Testcontainers.Internals.Parsers
{
  using DotNet.Testcontainers.Images;

  internal sealed class MatchImageTag : MatchImage
  {
    public MatchImageTag() : base($@"{Part}\:{Part}") // bar:1.0.0
    {
    }

    protected override IDockerImage Match(params string[] matches)
    {
      return new DockerImage(string.Empty, matches[0], matches[1]);
    }
  }
}
