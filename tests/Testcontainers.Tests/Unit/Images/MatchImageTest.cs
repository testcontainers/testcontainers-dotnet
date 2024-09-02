using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Tests.Fixtures;
using Xunit;

namespace DotNet.Testcontainers.Tests.Unit
{
  public class MatchImageTest
  {
    [Theory]
    [ClassData(typeof(MatchImageFixture))]
    public void MatchTest(string image, DockerImage expected)
    {
      var result = MatchImage.Match(image);
      Assert.Equivalent(expected, result);
    }
  }
}
