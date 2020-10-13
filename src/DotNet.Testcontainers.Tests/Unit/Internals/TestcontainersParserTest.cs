namespace DotNet.Testcontainers.Tests.Unit.Internals
{
  using System;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class TestcontainersParserTest
  {
    public class ParseDockerImage
    {
      [Fact]
      public void ShouldThrowArgumentNullExceptionWhenInstantiateDockerImage()
      {
        Assert.Throws<ArgumentNullException>(() => new DockerImage((string)null));
        Assert.Throws<ArgumentNullException>(() => new DockerImage(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new DockerImage("fedora", null, null));
        Assert.Throws<ArgumentNullException>(() => new DockerImage("fedora", "httpd", null));
      }

      [Fact]
      public void ShouldThrowArgumentExceptionWhenInstantiateDockerImage()
      {
        Assert.Throws<ArgumentException>(() => new DockerImage(string.Empty));
        Assert.Throws<ArgumentException>(() => new DockerImage(string.Empty, string.Empty, string.Empty));
      }

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
