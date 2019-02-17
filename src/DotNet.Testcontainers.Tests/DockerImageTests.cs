namespace DotNet.Testcontainers.Tests
{
  using System.Net;
  using DotNet.Testcontainers.Builder;
  using DotNet.Testcontainers.Images;
  using Xunit;

  public class DockerImageTests
  {
    [Theory]
    [ClassData(typeof(DockerImageTestDataNameParser))]
    public void Test_DockerImageNameParser_WithValidImageNames_NoException(GenericImage expected, string fullName)
    {
      // Given
      var dockerImage = new GenericImage();

      // When
      dockerImage.Image = fullName;

      // Then
      Assert.Equal(expected.Repository, dockerImage.Repository);
      Assert.Equal(expected.Name, dockerImage.Name);
      Assert.Equal(expected.Tag, dockerImage.Tag);
    }

    [Fact]
    public void Test_DockerContainerStartStop_WithValidImage_NoException()
    {
      // Given
      var dockerImage = "alpine";

      // When
      var dockerContainer = new ContainerBuilder().WithImage(dockerImage).Build();

      // Then
      dockerContainer.Pull();
      dockerContainer.Run();
      dockerContainer.Start();
      dockerContainer.Stop();
    }

    [Fact]
    public void Test_DockerContainerPortBindings_WithValidImage_NoException()
    {
      // Given
      var isAvailable = false;

      var port = 80;

      var dockerImage = new GenericImage("nginx");

      // When
      var dockerContainer = new ContainerBuilder()
        .WithImage(dockerImage)
        .WithPortBindings(port)
        .Build();

      // Then
      dockerContainer.Pull();
      dockerContainer.Run();
      dockerContainer.Start();

      var request = WebRequest.Create($"http://localhost:{port}");

      var response = (HttpWebResponse)request.GetResponse();

      dockerContainer.Stop();

      isAvailable = response != null && response.StatusCode == HttpStatusCode.OK;

      Assert.True(isAvailable, $"nginx port {port} is not available.");
    }
  }
}
