namespace DotNet.Testcontainers.Tests
{
  using DotNet.Testcontainers.Images;
  using Xunit;

  public class DockerImageTestDataNameParser : TheoryData<GenericImage, string>
  {
    public DockerImageTestDataNameParser()
    {
      this.Add(new GenericImage("foo", "bar", "1.0.0"), "foo/bar:1.0.0");
      this.Add(new GenericImage("foo", "bar", "latest"), "foo/bar:latest");
      this.Add(new GenericImage(string.Empty, "bar", "1.0.0"), "bar:1.0.0");
      this.Add(new GenericImage(string.Empty, "bar", "latest"), "bar:latest");
    }
  }
}
