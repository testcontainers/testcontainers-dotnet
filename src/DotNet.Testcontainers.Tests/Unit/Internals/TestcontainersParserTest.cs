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
        // Given
        var expected = serializable.Image;

        // When
        IDockerImage dockerImage = new DockerImage(fullName);

        // Then
        Assert.Equal(expected.Repository, dockerImage.Repository);
        Assert.Equal(expected.Name, dockerImage.Name);
        Assert.Equal(expected.Tag, dockerImage.Tag);
        Assert.Equal(expected.FullName, dockerImage.FullName);
      }
    }
  }
}
