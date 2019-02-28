namespace DotNet.Testcontainers.Tests
{
  using System;
  using System.IO;
  using System.Net;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core;
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
      public async Task QueryNotExistingDockerImageById()
      {
        Assert.False(await MetaDataClientImages.Instance.ExistsWithIdAsync(string.Empty));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerById()
      {
        Assert.False(await MetaDataClientContainers.Instance.ExistsWithIdAsync(string.Empty));
      }

      [Fact]
      public async Task QueryNotExistingDockerImageByName()
      {
        Assert.False(await MetaDataClientImages.Instance.ExistsWithNameAsync(string.Empty));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerByName()
      {
        var a = await MetaDataClientContainers.Instance.ExistsWithNameAsync(string.Empty);
        Assert.False(a);
      }

      [Fact]
      public async Task QueryContainerInformationOfRunningContainer()
      {
        // Given
        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("alpine");

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();

          Assert.NotEmpty(testcontainer.Name);
          Assert.NotEmpty(testcontainer.IPAddress);
          Assert.NotEmpty(testcontainer.MacAddress);
        }
      }

      [Fact]
      public void QueryContainerInformationOfStoppedContainer()
      {
        // Given
        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("alpine");

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          Assert.Throws<InvalidOperationException>(() => testcontainer.Name);
        }
      }
    }

    public class With
    {
      [Fact]
      public async Task Finalizer()
      {
        // Given
        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("alpine")
          .WithLabel("alpine", "latest");

        // Then
        var testcontainer = testcontainersBuilder.Build();
        await testcontainer.StartAsync();
        await testcontainer.StopAsync();
      }

      [Fact]
      public async Task Disposable()
      {
        // Given
        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("alpine");

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }
      }

      [Fact]
      public async Task GeneratedContainerName()
      {
        // Given
        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("alpine");

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.NotEmpty(testcontainer.Name);
        }
      }

      [Fact]
      public async Task SpecifiedContainerName()
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
          await testcontainer.StartAsync();
          Assert.Equal(name, testcontainer.Name);
        }
      }

      [Fact]
      public async Task ExposedPorts()
      {
        // Given
        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("alpine")
          .WithExposedPort(80);

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
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
        List(http, https).Iter(async port =>
        {
          using (var testcontainer = port.Map(nginx.WithPortBinding).Build())
          {
            await testcontainer.StartAsync();

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
      public async Task VolumeAndCommand()
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
          await testcontainer.StartAsync();
        }

        Assert.True(File.Exists($"{TempDir}/{file}"), $"{file} does not exist.");
      }

      [Fact]
      public async Task VolumeAndEnvironment()
      {
        // Given
        var target = "tmp";

        var file = "dayOfWeek";

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
          await testcontainer.StartAsync();
        }

        string text = File.ReadAllText($"{TempDir}/{file}");

        Assert.Equal(dayOfWeek, text);
      }
    }

    public class Strategy
    {
      [Fact]
      public async Task WaitWhile()
      {
        await WaitStrategy.WaitWhile(() =>
        {
          return false;
        });
      }

      [Fact]
      public async Task WaitUntil()
      {
        await WaitStrategy.WaitUntil(() =>
        {
          return true;
        });
      }

      [Fact]
      public async Task WaitWhileTimeout()
      {
        await Assert.ThrowsAsync<TimeoutException>(async () =>
        {
          await WaitStrategy.WaitWhile(
          () =>
          {
            return Wait100ms(true);
          }, timeout: 5);
        });
      }

      [Fact]
      public async Task WaitUntilTimeout()
      {
        await Assert.ThrowsAsync<TimeoutException>(async () =>
        {
          await WaitStrategy.WaitUntil(
          () =>
          {
            return Wait100ms(false);
          }, timeout: 5);
        });
      }

      private static bool Wait100ms(bool value)
      {
        Task.Delay(100);
        return value;
      }
    }
  }
}
