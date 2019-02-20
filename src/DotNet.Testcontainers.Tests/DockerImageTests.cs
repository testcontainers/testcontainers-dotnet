namespace DotNet.Testcontainers.Tests
{
  using System.Net;
  using DotNet.Testcontainers.Builder;
  using DotNet.Testcontainers.Images;
  using Xunit;
  using static LanguageExt.Prelude;

  public class DockerImageTests
  {
    [Theory]
    [ClassData(typeof(DockerImageTestDataNameParser))]
    public void Test_DockerImageNameParser_WithValidImageNames_NoException(IDockerImage expected, string fullName)
    {
      // Given
      var dockerImage = new TestcontainersImage();

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
      // Then
      using (var dockerContainer = new TestcontainersBuilder().WithImage(dockerImage).Build())
      {
        dockerContainer.Start();
      }
    }

    [Fact]
    public void Test_DockerContainerPortBindings_WithValidImage_NoException()
    {
      // Given
      var http = Tuple(80, 80);
      var https = Tuple(443, 80);

      // When
      var nginx = new TestcontainersBuilder().WithImage("nginx");

      // Then
      List(http, https).Iter(port =>
      {
        using (var dockerContainer = port.Map(nginx.WithPortBinding).Build())
        {
          dockerContainer.Start();

          var request = WebRequest.Create($"http://localhost:{port.Item1}");

          var response = (HttpWebResponse)request.GetResponse();

          if (response == null || response.StatusCode != HttpStatusCode.OK)
          {
            Assert.True(false, $"nginx port {port.Item1} is not available.");
          }
        }
      });
    }

    [Fact]
    public void Test_DockerContainerName_WithoutName_NoException()
    {
      // When
      var dockerContainer = new TestcontainersBuilder()
        .WithImage("alpine")
        .Build();

      dockerContainer.Start();
      dockerContainer.Dispose();

      // Then
      Assert.NotEmpty(dockerContainer.Name);
    }

    [Fact]
    public void Test_DockerContainerName_WithName_NoException()
    {
      // Given
      var name = "foo";

      // When
      var dockerContainer = new TestcontainersBuilder()
        .WithImage("alpine")
        .WithName(name)
        .Build();

      dockerContainer.Start();
      dockerContainer.Dispose();

      // Then
      Assert.Equal(name, dockerContainer.Name);
    }
  }
}
