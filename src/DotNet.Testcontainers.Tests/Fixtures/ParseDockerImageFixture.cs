namespace DotNet.Testcontainers.Tests.Fixtures
{
  using DotNet.Testcontainers.Core.Images;
  using Xunit;

  public class ParseDockerImageFixture : TheoryData<ParseDockerImageFixtureSerializable, string>
  {
    public ParseDockerImageFixture()
    {
      this.Add(new ParseDockerImageFixtureSerializable(new TestcontainersImage("baz/foo", "bar", "1.0.0")), "baz/foo/bar:1.0.0");
      this.Add(new ParseDockerImageFixtureSerializable(new TestcontainersImage("foo", "bar", "1.0.0")), "foo/bar:1.0.0");
      this.Add(new ParseDockerImageFixtureSerializable(new TestcontainersImage("foo", "bar", string.Empty)), "foo/bar:latest");
      this.Add(new ParseDockerImageFixtureSerializable(new TestcontainersImage(string.Empty, "bar", "1.0.0")), "bar:1.0.0");
      this.Add(new ParseDockerImageFixtureSerializable(new TestcontainersImage(string.Empty, "bar", string.Empty)), "bar:latest");
    }
  }
}
