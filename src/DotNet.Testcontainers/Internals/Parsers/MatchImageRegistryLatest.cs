namespace DotNet.Testcontainers.Internals.Parsers
{
  using DotNet.Testcontainers.Images;

  internal sealed class MatchImageRegistryLatest : MatchImage
  {
    public MatchImageRegistryLatest() : base($@"{Part}\/{Part}\/{Part}") // Matches baz/foo/bar
    {
    }

    protected override IDockerImage Match(params string[] matches)
    {
      return new DockerImage($"{matches[0]}/{matches[1]}", matches[2], string.Empty);
    }
  }
}
