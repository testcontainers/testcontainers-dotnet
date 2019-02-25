namespace DotNet.Testcontainers.Tests
{
  using System;
  using System.IO;
  using System.Net;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Images;
  using Xunit;
  using static LanguageExt.Prelude;

  public static class TestcontainersTests
  {
    private static readonly string TempDir = Environment.GetEnvironmentVariable("AGENT_TEMPDIRECTORY") ?? "."; // We cannot use `Path.GetTempPath()` on macOS, see: https://github.com/common-workflow-language/cwltool/issues/328

    public class ParseDockerImageName
    {
      [Theory]
      [ClassData(typeof(DockerImageTestDataNameParser))]
      public void WhenImageNameGetsAssigend(IDockerImage expected, string fullName)
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
    }

    public class AccessDockerInformation
    {
      [Fact]
      public void QueryNotExistingDockerImageById()
      {
        Assert.False(TestcontainersClient.Instance.ExistImageById(string.Empty));
      }

      [Fact]
      public void QueryNotExistingDockerContainerById()
      {
        Assert.False(TestcontainersClient.Instance.ExistContainerById(string.Empty));
      }

      [Fact]
      public void QueryNotExistingDockerImageByName()
      {
        Assert.False(TestcontainersClient.Instance.ExistImageByName(string.Empty));
      }

      [Fact]
      public void QueryNotExistingDockerContainerByName()
      {
        Assert.False(TestcontainersClient.Instance.ExistContainerByName(string.Empty));
      }
    }

    public class With
    {
      [Fact]
      public void SimpleImage()
      {
        // Given
        var dockerImage = "alpine";

        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage(dockerImage);

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          testcontainer.Start();
        }
      }

      [Fact]
      public void GeneratedContainerName()
      {
        // Given
        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("alpine");

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          testcontainer.Start();
          Assert.NotEmpty(testcontainer.Name);
        }
      }

      [Fact]
      public void SpecifiedContainerName()
      {
        // Given
        var name = "/alpine";

        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("alpine")
          .WithName(name);

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          testcontainer.Start();
          Assert.Equal(name, testcontainer.Name);
        }
      }

      [Fact]
      public void ExposedPorts()
      {
        // Given
        var name = "alpine";

        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("alpine")
          .WithExposedPort(80)
          .WithName(name);

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          testcontainer.Start();
        }
      }

      [Fact]
      public void PortBindingsHttpAndHttps()
      {
        // Given
        var http = Tuple(80, 80);
        var https = Tuple(443, 80);

        // When
        var nginx = new TestcontainersBuilder()
          .WithImage("nginx");

        // Then
        List(http, https).Iter(port =>
        {
          using (var testcontainer = port.Map(nginx.WithPortBinding).Build())
          {
            testcontainer.Start();

            var request = WebRequest.Create($"http://localhost:{port.Item1}");

            var response = (HttpWebResponse)request.GetResponse();

            var isAvailable = Optional(response).Match(
              Some: value => value.StatusCode == HttpStatusCode.OK,
              None: () => false);

            Assert.True(isAvailable, $"nginx port {port.Item1} is not available.");
          }
        });
      }

      [Fact]
      public void VolumeAndCommand()
      {
        // Given
        var target = "tmp";

        var file = "hostname";

        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("nginx")
          .WithMount(TempDir, $"/{target}")
          .WithCommand("/bin/bash", "-c", $"hostname > /{target}/{file}");

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          testcontainer.Start();
        }

        Assert.True(File.Exists($"{TempDir}/{file}"), $"{file} does not exist.");
      }

      [Fact]
      public void VolumeAndEnvironment()
      {
        // Given
        var target = "tmp";

        var file = "today";

        var dayOfWeek = DateTime.Now.DayOfWeek.ToString();

        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("nginx")
          .WithMount(TempDir, $"/{target}")
          .WithEnvironment("dayOfWekk", dayOfWeek)
          .WithCommand("/bin/bash", "-c", $"printf $dayOfWekk > /{target}/{file}");

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          testcontainer.Start();
        }

        string text = File.ReadAllText($"{TempDir}/{file}");

        Assert.Equal(dayOfWeek, text);
      }
    }
  }
}
