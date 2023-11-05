namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class TestcontainersImageTest
  {
    [Fact]
    public void ShouldThrowArgumentNullExceptionWhenInstantiateDockerImage()
    {
      Assert.Throws<ArgumentException>(() => new DockerImage((string)null));
      Assert.Throws<ArgumentException>(() => new DockerImage(null, null));
      Assert.Throws<ArgumentException>(() => new DockerImage("fedora", null));
    }

    [Fact]
    public void ShouldThrowArgumentExceptionWhenInstantiateDockerImage()
    {
      Assert.Throws<ArgumentException>(() => new DockerImage(string.Empty));
      Assert.Throws<ArgumentException>(() => new DockerImage(string.Empty, string.Empty, string.Empty));
    }

    [Theory]
    [InlineData("Bar")]
    [InlineData("Bar:latest")]
    [InlineData("Foo/Bar")]
    [InlineData("Foo/Bar:latest")]
    [InlineData("Foo/bar")]
    [InlineData("Foo/bar:latest")]
    public void ShouldThrowArgumentExceptionIfImageNameHasUppercaseCharacters(string image)
    {
      Assert.Throws<ArgumentException>(() => new DockerImage(image));
    }

    [Theory]
    [InlineData("bar:LATEST")]
    [InlineData("foo/bar:LATEST")]
    public void ShouldNotThrowArgumentExceptionIfImageTagHasUppercaseCharacters(string image)
    {
      var exception = Record.Exception(() => new DockerImage(image));
      Assert.Null(exception);
    }

    [Theory]
    [ClassData(typeof(DockerImageFixture))]
    public void WhenImageNameGetsAssigned(DockerImageFixtureSerializable serializable, string fullName)
    {
      // Given
      var expected = serializable.Image;

      // When
      IImage dockerImage = new DockerImage(fullName);

      // Then
      Assert.Equal(expected.Repository, dockerImage.Repository);
      Assert.Equal(expected.Name, dockerImage.Name);
      Assert.Equal(expected.Tag, dockerImage.Tag);
      Assert.Equal(expected.FullName, dockerImage.FullName);
    }
  }
}
