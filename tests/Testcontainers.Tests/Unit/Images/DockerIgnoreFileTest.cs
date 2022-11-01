namespace DotNet.Testcontainers.Tests.Unit
{
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class DockerIgnoreFileTest
  {
    [Theory]
    [ClassData(typeof(DockerIgnoreFileFixture))]
    public void AlwaysAcceptsDockerfileAndDockerignoreFiles(IgnoreFile ignoreFile, string path, bool expected)
    {
      Assert.Equal(expected, ignoreFile.Accepts(path));
    }
  }
}
