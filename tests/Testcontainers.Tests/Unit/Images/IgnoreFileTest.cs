namespace DotNet.Testcontainers.Tests.Unit
{
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class IgnoreFileTest
  {
    [Theory]
    [ClassData(typeof(IgnoreFileFixture))]
    public void AcceptOrDenyNonRecursivePatterns(IgnoreFile ignoreFile, string path, bool expected)
    {
      Assert.Equal(expected, ignoreFile.Accepts(path));
    }
  }
}
