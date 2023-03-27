namespace DotNet.Testcontainers.Tests.Fixtures
{
  using DotNet.Testcontainers.Images;
  using Xunit;

  public sealed class DockerImageFixture : TheoryData<DockerImageFixtureSerializable, string>
  {
    public DockerImageFixture()
    {
      Add(new DockerImageFixtureSerializable(new DockerImage("baz/foo", "bar", "1.0.0")), "baz/foo/bar:1.0.0");
      Add(new DockerImageFixtureSerializable(new DockerImage("baz/foo", "bar", string.Empty)), "baz/foo/bar");
      Add(new DockerImageFixtureSerializable(new DockerImage("baz/foo", "bar", string.Empty)), "baz/foo/bar:latest");
      Add(new DockerImageFixtureSerializable(new DockerImage("foo", "bar", "1.0.0")), "foo/bar:1.0.0");
      Add(new DockerImageFixtureSerializable(new DockerImage("foo", "bar", string.Empty)), "foo/bar");
      Add(new DockerImageFixtureSerializable(new DockerImage("foo", "bar", string.Empty)), "foo/bar:latest");
      Add(new DockerImageFixtureSerializable(new DockerImage(string.Empty, "bar", "1.0.0")), "bar:1.0.0");
      Add(new DockerImageFixtureSerializable(new DockerImage(string.Empty, "bar", string.Empty)), "bar:latest");
      Add(new DockerImageFixtureSerializable(new DockerImage("myregistry.azurecr.io/baz/foo", "bar", "1.0.0")), "myregistry.azurecr.io/baz/foo/bar:1.0.0");
      Add(new DockerImageFixtureSerializable(new DockerImage("myregistry.azurecr.io/baz/foo", "bar", string.Empty)), "myregistry.azurecr.io/baz/foo/bar");
      Add(new DockerImageFixtureSerializable(new DockerImage("myregistry.azurecr.io/baz/foo", "bar", string.Empty)), "myregistry.azurecr.io/baz/foo/bar:latest");
      Add(new DockerImageFixtureSerializable(new DockerImage("fedora", "httpd", "version1.0.test")), "fedora/httpd:version1.0.test");
      Add(new DockerImageFixtureSerializable(new DockerImage("fedora", "httpd", "version1.0")), "fedora/httpd:version1.0");
      Add(new DockerImageFixtureSerializable(new DockerImage("myregistryhost:5000/fedora", "httpd", "version1.0")), "myregistryhost:5000/fedora/httpd:version1.0");
    }
  }
}
