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
    [InlineData("baz/.foo/bar:1.0.0")]
    [InlineData("baz/:foo/bar:1.0.0")]
    public void ShouldThrowArgumentExceptionIfImageIsInvalidReferenceFormat(string image)
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
    public void WhenImageNameGetsAssigned(DockerImageFixtureSerializable serializable, string image, string fullName)
    {
      // Given
      var expected = serializable.Image;

      // When
      IImage actual = new DockerImage(image);

      // Then
      Assert.Equal(expected.Repository, actual.Repository);
      Assert.Equal(expected.Registry, actual.Registry);
      Assert.Equal(expected.Tag, actual.Tag);
      Assert.Equal(expected.Digest, actual.Digest);
      Assert.Equal(expected.FullName, actual.FullName);
      Assert.Equal(fullName, actual.FullName);
    }

    [Fact]
    public void MatchLatestOrNightly_TagIsLatest_ReturnsTrue()
    {
      // Given
      IImage dockerImage = new DockerImage("foo:latest");

      // When
      var result = dockerImage.MatchLatestOrNightly();

      // Then
      Assert.True(result);
    }

    [Fact]
    public void MatchLatestOrNightly_TagIsNightly_ReturnsTrue()
    {
      // Given
      IImage dockerImage = new DockerImage("foo:nightly");

      // When
      var result = dockerImage.MatchLatestOrNightly();

      // Then
      Assert.True(result);
    }

    [Fact]
    public void MatchLatestOrNightly_TagIsNeither_ReturnsFalse()
    {
      // Given
      IImage dockerImage = new DockerImage("foo:1.0.0");

      // When
      var result = dockerImage.MatchLatestOrNightly();

      // Then
      Assert.False(result);
    }

    [Theory]
    [InlineData("foo:2", 2, 0, -1, -1)]
    [InlineData("foo:2-variant", 2, 0, -1, -1)]
    [InlineData("foo:2.3", 2, 3, -1, -1)]
    [InlineData("foo:2.3-variant", 2, 3, -1, -1)]
    [InlineData("foo:2.3.4", 2, 3, 4, -1)]
    [InlineData("foo:2.3.4-variant", 2, 3, 4, -1)]
    public void MatchVersion_ReturnsTrue_WhenVersionMatchesPredicate(string image, int major, int minor, int build, int revision)
    {
      // Given
      Predicate<Version> predicate = v => v.Major == major && v.Minor == minor && v.Build == build && v.Revision == revision;
      IImage dockerImage = new DockerImage(image);

      // When
      var result = dockerImage.MatchVersion(predicate);

      // Then
      Assert.True(result);
    }

    [Fact]
    public void MatchVersion_ReturnsFalse_WhenVersionDoesNotMatchPredicate()
    {
      // Given
      Predicate<Version> predicate = v => v.Major == 0 && v.Minor == 0 && v.Build == 1;
      IImage dockerImage = new DockerImage("foo:1.0.0");

      // When
      var result = dockerImage.MatchVersion(predicate);

      // Then
      Assert.False(result);
    }
  }
}
