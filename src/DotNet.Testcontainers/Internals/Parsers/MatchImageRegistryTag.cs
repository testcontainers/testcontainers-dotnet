namespace DotNet.Testcontainers.Internals.Parsers
{
  using DotNet.Testcontainers.Images;

  internal sealed class MatchImageRegistryTag : MatchImage
  {
    public MatchImageRegistryTag() : base(@"^([\w][\w\.\-:/]+)/([\w][\w\.\-]+):([\w][\w\.\-]{0,127})$") // Matches baz/foo/bar:1.0.0
    {
    }

    protected override IDockerImage Match(params string[] matches)
    {
      return new DockerImage(matches[0], matches[1], matches[2]);
    }
  }
}
