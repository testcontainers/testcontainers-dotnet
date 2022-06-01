namespace DotNet.Testcontainers.Tests.Unit.Builders;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Xunit;

[Collection(nameof(ParallelizationDisabled))]
public class DockerHubImagePrefixTest
{
  [Theory]
  [InlineData("my.proxy.com", "bar", "my.proxy.com/bar:latest")]
  [InlineData("my.proxy.com", "bar:latest", "my.proxy.com/bar:latest")]
  [InlineData("my.proxy.com", "bar:1.0.0", "my.proxy.com/bar:1.0.0")]
  [InlineData("my.proxy.com/my-path", "bar:1.0.0", "my.proxy.com/my-path/bar:1.0.0")]
  [InlineData("my.proxy.com:443", "bar:1.0.0", "my.proxy.com:443/bar:1.0.0")]
  [InlineData("my.proxy.com", "foo/bar:1.0.0", "my.proxy.com/foo/bar:1.0.0")]
  [InlineData("my.proxy.com/my-path", "foo/bar:1.0.0", "my.proxy.com/my-path/foo/bar:1.0.0")]
  [InlineData("my.proxy.com:443", "foo/bar:1.0.0", "my.proxy.com:443/foo/bar:1.0.0")]
  [InlineData("my.proxy.com:443/my-path", "foo/bar:1.0.0", "my.proxy.com:443/my-path/foo/bar:1.0.0")]
  [InlineData("my.proxy.com", "myregistry.azurecr.io/foo/bar:1.0.0", "myregistry.azurecr.io/foo/bar:1.0.0")]
  [InlineData("my.proxy.com", "myregistry.azurecr.io:443/foo/bar:1.0.0", "myregistry.azurecr.io:443/foo/bar:1.0.0")]
  public void ImageNameFromStringIsPrefixedWhenPrefixIsSet(string prefix, string originalImageName, string expectedName)
  {
    TestcontainersSettings.DockerHubImagePrefix = prefix;

    var container = new TestcontainersBuilder<TestcontainersContainer>()
      .WithImage(originalImageName)
      .Build();

    Assert.Equal(expectedName, container.ImageName);
  }

  [Theory]
  [InlineData("my.proxy.com", "bar", "my.proxy.com/bar:latest")]
  [InlineData("my.proxy.com", "bar:latest", "my.proxy.com/bar:latest")]
  [InlineData("my.proxy.com", "bar:1.0.0", "my.proxy.com/bar:1.0.0")]
  [InlineData("my.proxy.com/my-path", "bar:1.0.0", "my.proxy.com/my-path/bar:1.0.0")]
  [InlineData("my.proxy.com:443", "bar:1.0.0", "my.proxy.com:443/bar:1.0.0")]
  [InlineData("my.proxy.com", "foo/bar:1.0.0", "my.proxy.com/foo/bar:1.0.0")]
  [InlineData("my.proxy.com/my-path", "foo/bar:1.0.0", "my.proxy.com/my-path/foo/bar:1.0.0")]
  [InlineData("my.proxy.com:443", "foo/bar:1.0.0", "my.proxy.com:443/foo/bar:1.0.0")]
  [InlineData("my.proxy.com:443/my-path", "foo/bar:1.0.0", "my.proxy.com:443/my-path/foo/bar:1.0.0")]
  [InlineData("my.proxy.com", "myregistry.azurecr.io/foo/bar:1.0.0", "myregistry.azurecr.io/foo/bar:1.0.0")]
  [InlineData("my.proxy.com", "myregistry.azurecr.io:443/foo/bar:1.0.0", "myregistry.azurecr.io:443/foo/bar:1.0.0")]
  public void ImageNameFromClassIsPrefixedWhenPrefixIsSet(string prefix, string originalImageName, string expectedName)
  {
    TestcontainersSettings.DockerHubImagePrefix = prefix;

    var container = new TestcontainersBuilder<TestcontainersContainer>()
      .WithImage(new DockerImage(originalImageName))
      .Build();

    Assert.Equal(expectedName, container.ImageName);
  }

  [Fact]
  public void ImageNameIsNotPrefixedWhenPrefixIsNotSet()
  {
    TestcontainersSettings.DockerHubImagePrefix = null;

    var container = new TestcontainersBuilder<TestcontainersContainer>()
      .WithImage("foo/bar:1.0.0")
      .Build();

    Assert.Equal("foo/bar:1.0.0", container.ImageName);
  }
}
