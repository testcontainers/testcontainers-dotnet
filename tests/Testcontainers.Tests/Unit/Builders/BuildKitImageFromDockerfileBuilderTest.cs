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
      var buildKitImageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder(CommonImages.DockerCli);

      // When
      var exception = Record.Exception(buildKitImageFromDockerfileBuilder.Build);

      // Then
      Assert.Null(exception);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenReuseIsEnabled()
    {
      // Given
      var buildKitImageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder(CommonImages.DockerCli).WithReuse(true);

      // When
      var exception = Assert.Throws<ArgumentException>(buildKitImageFromDockerfileBuilder.Build);

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
      var buildKitImageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder(CommonImages.DockerCli).WithSecret(secretId, "value");

      // When
      var exception = Assert.Throws<ArgumentException>(buildKitImageFromDockerfileBuilder.Build);

      // Then
      Assert.StartsWith($"The build secret id '{secretId}' must start with", exception.Message);
    }

    [Fact]
    public void ThrowsFileNotFoundExceptionWhenSecretFileDoesNotExist()
    {
      // Given
      var secretFilePath = Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"));

      var buildKitImageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder(CommonImages.DockerCli).WithSecret("mysecret", FilePath.Of(secretFilePath));

      // When
      var exception = Assert.Throws<FileNotFoundException>(buildKitImageFromDockerfileBuilder.Build);

      // Then
      Assert.Equal($"The build secret file '{secretFilePath}' does not exist.", exception.Message);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenSshIdIsInvalid()
    {
      // Given
      var buildKitImageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder(CommonImages.DockerCli).WithSsh("invalid id");

      // When
      var exception = Assert.Throws<ArgumentException>(buildKitImageFromDockerfileBuilder.Build);

      // Then
      Assert.StartsWith("The SSH id 'invalid id' must start with", exception.Message);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenSshPathIsMissing()
    {
      // Given
      var buildKitImageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder(CommonImages.DockerCli).WithSsh("default");

      // When
      var exception = Assert.Throws<ArgumentException>(buildKitImageFromDockerfileBuilder.Build);

      // Then
      Assert.StartsWith("The SSH id 'default' does not set a path.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowsArgumentExceptionWhenSshPathIsEmpty(string sshPath)
    {
      // Given
      var buildKitImageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder(CommonImages.DockerCli).WithSsh("default", sshPath);

      // When
      var exception = Assert.Throws<ArgumentException>(buildKitImageFromDockerfileBuilder.Build);

      // Then
      Assert.StartsWith("The SSH id 'default' does not set a path.", exception.Message);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenSshPathContainsComma()
    {
      // Given
      var sshPath = Path.Combine(TestSession.TempDirectoryPath, "ssh,agent.sock");

      var buildKitImageFromDockerfileBuilder = new BuildKitImageFromDockerfileBuilder(CommonImages.DockerCli).WithSsh("default", sshPath);

      // When
      var exception = Assert.Throws<ArgumentException>(buildKitImageFromDockerfileBuilder.Build);

      // Then
      Assert.StartsWith($"The SSH path '{sshPath}' cannot contain a comma", exception.Message);
    }
  }
}
