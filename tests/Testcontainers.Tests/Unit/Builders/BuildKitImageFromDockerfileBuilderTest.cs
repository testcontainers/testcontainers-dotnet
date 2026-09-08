namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using Xunit;

  public sealed class BuildKitImageFromDockerfileBuilderTest
  {
    [Fact]
    public void BuildsWithDefaultConfiguration()
    {
      // Given
      var imageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder();

      // When
      var exception = Record.Exception(() => imageFromDockerfileBuilder.Build());

      // Then
      Assert.Null(exception);
    }

    [Fact]
    public void BuildsWithCustomDockerCliImage()
    {
      // Given
      var imageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder(CommonImages.DockerCli);

      // When
      var exception = Record.Exception(() => imageFromDockerfileBuilder.Build());

      // Then
      Assert.Null(exception);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenReuseIsEnabled()
    {
      // Given
      var imageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder().WithReuse(true);

      // When
      var exception = Assert.Throws<ArgumentException>(() => imageFromDockerfileBuilder.Build());

      // Then
      Assert.StartsWith("Building an image does not support the reuse feature.", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("-invalid")]
    [InlineData("invalid id")]
    [InlineData("../invalid")]
    [InlineData("invalid/id")]
    public void ThrowsArgumentExceptionWhenSecretIdIsInvalid(string secretId)
    {
      // Given
      var imageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder().WithSecret(secretId, "value");

      // When
      var exception = Assert.Throws<ArgumentException>(() => imageFromDockerfileBuilder.Build());

      // Then
      Assert.StartsWith($"The build secret id '{secretId}' must start with", exception.Message);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenSecretIdIsNotUnique()
    {
      // Given
      var imageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder()
        .WithSecret("mysecret", "value")
        .WithSecret("mysecret", new FileInfo("value"));

      // When
      var exception = Assert.Throws<ArgumentException>(() => imageFromDockerfileBuilder.Build());

      // Then
      Assert.StartsWith("The build secret id 'mysecret' is set more than once.", exception.Message);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenSshAgentIdIsInvalid()
    {
      // Given
      var imageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder().WithSshAgent("invalid id");

      // When
      var exception = Assert.Throws<ArgumentException>(() => imageFromDockerfileBuilder.Build());

      // Then
      Assert.StartsWith("The SSH agent id 'invalid id' must start with", exception.Message);
    }
  }
}
