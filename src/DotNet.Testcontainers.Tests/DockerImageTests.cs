namespace DotNet.Testcontainers.Tests
{
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
    public void Test_Start()
    {
      using (var dockerImage = new ContainerBuilder().WithImage("alpine").Build())
      {
        dockerImage.Run();
        dockerImage.Start();
        dockerImage.Stop();
        dockerImage.Start();
      }
    }
  }
}
