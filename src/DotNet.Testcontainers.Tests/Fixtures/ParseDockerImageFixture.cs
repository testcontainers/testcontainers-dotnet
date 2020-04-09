namespace DotNet.Testcontainers.Tests.Fixtures
{
  using DotNet.Testcontainers.Images;
  using Xunit;

  public class ParseDockerImageFixture : TheoryData<ParseDockerImageFixtureSerializable, string>
  {
    public ParseDockerImageFixture()
    {
      this.Add(new ParseDockerImageFixtureSerializable(new DockerImage("baz/foo", "bar", "1.0.0")), "baz/foo/bar:1.0.0");
      this.Add(new ParseDockerImageFixtureSerializable(new DockerImage("baz/foo", "bar", string.Empty)), "baz/foo/bar");
      this.Add(new ParseDockerImageFixtureSerializable(new DockerImage("baz/foo", "bar", string.Empty)), "baz/foo/bar:latest");
      this.Add(new ParseDockerImageFixtureSerializable(new DockerImage("foo", "bar", "1.0.0")), "foo/bar:1.0.0");
      this.Add(new ParseDockerImageFixtureSerializable(new DockerImage("foo", "bar", string.Empty)), "foo/bar");
      this.Add(new ParseDockerImageFixtureSerializable(new DockerImage("foo", "bar", string.Empty)), "foo/bar:latest");
      this.Add(new ParseDockerImageFixtureSerializable(new DockerImage(string.Empty, "bar", "1.0.0")), "bar:1.0.0");
      this.Add(new ParseDockerImageFixtureSerializable(new DockerImage(string.Empty, "bar", string.Empty)), "bar:latest");
    }
  }
}
