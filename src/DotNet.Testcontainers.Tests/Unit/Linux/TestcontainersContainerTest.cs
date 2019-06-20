namespace DotNet.Testcontainers.Tests.Unit.Linux
{
  using System;
  using System.IO;
  using System.Net;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Containers;
  using DotNet.Testcontainers.Core.Wait;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class TestcontainersContainerTest
  {
    private static readonly string tempDir = Environment.GetEnvironmentVariable("AGENT_TEMPDIRECTORY") ?? "."; // We cannot use `Path.GetTempPath()` on macOS, see: https://github.com/common-workflow-language/cwltool/issues/328

    public class With
    {
      [Fact]
      public void IsLinuxEngineEnabled()
      {
        Assert.False(DockerHostConfiguration.IsWindowsEngineEnabled);
      }

      [Fact]
      public async Task Finalizer()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithCleanUp(true)
          .WithImage("alpine")
          .WithLabel("alpine", "latest");

        // When
        // Then
        var testcontainer = testcontainersBuilder.Build();
        await testcontainer.StartAsync();
        await testcontainer.StopAsync();
      }

      [Fact]
      public async Task Disposable()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine");

        // When
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
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine");

        // When
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
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithExposedPort(80);

        // When
        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }
      }

      [Fact]
      public async Task PortBindingsHttpAndHttps()
      {
        // Given
        var http = new { From = 80, To = 80 };

        var https = new { From = 443, To = 80 };

        var nginx = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx");

        // When
        // Then
        foreach (var port in new[] { http, https })
        {
          using (var testcontainer = nginx
            .WithPortBinding(port.From, port.To)
            .WithWaitStrategy(Wait.UntilPortsAreAvailable(port.To))
            .Build())
          {
            await testcontainer.StartAsync();

            var request = WebRequest.Create($"http://localhost:{port.From}");

            var response = (HttpWebResponse)request.GetResponse();

            var isAvailable = response != null && response.StatusCode == HttpStatusCode.OK;

            Assert.True(isAvailable, $"nginx port {port.From} is not available.");
          }
        }
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
          .WithMount(tempDir, $"/{target}")
          .WithWaitStrategy(Wait.UntilFilesExists($"{tempDir}/{file}"))
          .WithCommand("/bin/bash", "-c", $"hostname > /{target}/{file}");

        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }

        // Then
        Assert.True(File.Exists($"{tempDir}/{file}"), $"{file} does not exist.");
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
          .WithMount(tempDir, $"/{target}")
          .WithEnvironment("dayOfWeek", dayOfWeek)
          .WithWaitStrategy(Wait.UntilFilesExists($"{tempDir}/{file}"))
          .WithCommand("/bin/bash", "-c", $"printf $dayOfWeek > /{target}/{file}");

        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }

        // Then
        Assert.Equal(dayOfWeek, File.ReadAllText($"{tempDir}/{file}"));
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

          using (var testcontainer = testcontainersBuilder.Build())
          {
            await testcontainer.StartAsync();
          }

          output.Stdout.Position = 0;
          output.Stderr.Position = 0;

          // Then
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
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithWaitStrategy(new WaitStrategyFixture());

        // When
        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }
      }
    }
  }
}
