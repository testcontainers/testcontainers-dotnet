namespace DotNet.Testcontainers.Tests.Unit.Images
{
  using DotNet.Testcontainers.Images.Archives;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public class IgnoreFileTest
  {
    [Theory]
    [ClassData(typeof(IgnoreFileTestFixture))]
    internal void AcceptOrDenyNonRecursivePatterns(IgnoreFile ignoreFile, string path, bool expected)
    {
      Assert.Equal(expected, ignoreFile.Accepts(path));
    }
  }
}
