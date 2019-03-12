namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using System.Net;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Containers;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;
  using static LanguageExt.Prelude;

  public static class TestcontainersTest
  {
    private static readonly string TempDir = Environment.GetEnvironmentVariable("AGENT_TEMPDIRECTORY") ?? "."; // We cannot use `Path.GetTempPath()` on macOS, see: https://github.com/common-workflow-language/cwltool/issues/328

    public class With
    {
      [Fact]
      public async Task Finalizer()
      {
        // Given
        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
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
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
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
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
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
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
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
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
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
        var nginx = new TestcontainersBuilder<TestcontainersContainer>()
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
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
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
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx")
          .WithMount(TempDir, $"/{target}")
          .WithEnvironment("dayOfWeek", dayOfWeek)
          .WithCommand("/bin/bash", "-c", $"printf $dayOfWeek > /{target}/{file}");

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }

        string text = File.ReadAllText($"{TempDir}/{file}");

        Assert.Equal(dayOfWeek, text);
      }

      [Fact]
      public async Task OutputConsumer()
      {
        // Given
        using (var output = new DefaultConsumerFixture())
        {
          // When
          var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("nginx")
            .WithOutputConsumer(output)
            .WithCommand("/bin/bash", "-c", "hostname > /dev/stdout && hostname > /dev/stderr");

          // Then
          using (var testcontainer = testcontainersBuilder.Build())
          {
            await testcontainer.StartAsync();
          }

          output.Stdout.Position = 0;
          output.Stderr.Position = 0;

          using (var streamReader = new StreamReader(output.Stdout))
          {
            Assert.NotEmpty(streamReader.ReadToEnd());
          }

          using (var streamReader = new StreamReader(output.Stderr))
          {
            Assert.NotEmpty(streamReader.ReadToEnd());
          }
        }
      }

      [Fact]
      public async Task WaitStrategy()
      {
        // Given
        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithWaitStrategy(new WaitStrategyFixture());

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }
      }
    }
  }
}
