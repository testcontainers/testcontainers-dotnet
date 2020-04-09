namespace DotNet.Testcontainers.Internals.Parsers
{
  using DotNet.Testcontainers.Images;

  internal sealed class MatchImageRegistryTag : MatchImage
  {
    public MatchImageRegistryTag() : base($@"{Part}\/{Part}\/{Part}\:{Part}") // Matches baz/foo/bar:1.0.0
    {
    }

    protected override IDockerImage Match(params string[] matches)
    {
      return new DockerImage($"{matches[0]}/{matches[1]}", matches[2], matches[3]);
    }
  }
}
