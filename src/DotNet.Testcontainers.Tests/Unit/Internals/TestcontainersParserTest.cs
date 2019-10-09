namespace DotNet.Testcontainers.Tests.Unit.Internals
{
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class TestcontainersParserTest
  {
    public class ParseDockerImage
    {
      [Theory]
      [ClassData(typeof(ParseDockerImageFixture))]
      public void WhenImageNameGetsAssigned(ParseDockerImageFixtureSerializable serializable, string fullName)
      {
        var expected = serializable.Image;

        // Given
        var dockerImage = new DockerImage(fullName);

        // When
        // Then
        Assert.Equal(expected.Repository, dockerImage.Repository);
        Assert.Equal(expected.Name, dockerImage.Name);
        Assert.Equal(expected.Tag, dockerImage.Tag);
      }
    }
  }
}
