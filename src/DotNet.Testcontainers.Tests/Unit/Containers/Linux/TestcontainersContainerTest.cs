namespace DotNet.Testcontainers.Tests.Unit.Containers.Linux
{
  using System;
  using System.IO;
  using System.Net;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Modules;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class TestcontainersContainerTest
  {
    private static readonly string TempDir = Environment.GetEnvironmentVariable("AGENT_TEMPDIRECTORY") ?? "."; // We cannot use `Path.GetTempPath()` on macOS, see: https://github.com/common-workflow-language/cwltool/issues/328

    public class With
    {
      [Fact]
      public async Task IsLinuxEngineEnabled()
      {
        Assert.False(await new TestcontainersClient().GetIsWindowsEngineEnabled());
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
        const string name = "/alpine";

        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithName(name);

        // Then
        using (IDockerContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.Equal(name, testcontainer.Name);
        }
      }

      [Fact]
      public async Task WorkingDirectory()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithWorkingDirectory("/tmp")
          .WithCommand("/bin/sh", "-c", "test -d /tmp && exit $? || exit $?");

        // When
        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.Equal(0, await testcontainer.GetExitCode());
        }
      }

      [Fact]
      public async Task Entrypoint()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint("/bin/sh", "-c", "exit 255");

        // When
        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.Equal(255, await testcontainer.GetExitCode());
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

        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx");

        // When
        // Then
        foreach (var port in new[] { http, https })
        {
          using (var testcontainer = testcontainersBuilder
            .WithPortBinding(port.From, port.To)
            .WithWaitStrategy(Wait.UntilPortsAreAvailable(port.To))
            .Build())
          {
            await testcontainer.StartAsync();

            var request = WebRequest.Create($"http://localhost:{port.From}");

            var response = (HttpWebResponse)request.GetResponse();

            var isAvailable = response.StatusCode == HttpStatusCode.OK;

            Assert.True(isAvailable, $"nginx port {port.From} is not available.");
          }
        }
      }

      [Fact]
      public async Task RandomHostPortBindings()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx")
          .WithPortBinding(80, true);

        // When
        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.NotEqual(0, testcontainer.GetMappedPublicPort(80));
        }
      }

      [Fact]
      public async Task VolumeAndCommand()
      {
        // Given
        const string target = "tmp";

        const string file = "hostname";

        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx")
          .WithMount(TempDir, $"/{target}")
          .WithWaitStrategy(Wait.UntilFilesExists($"{TempDir}/{file}"))
          .WithCommand("/bin/sh", "-c", $"hostname > /{target}/{file}");

        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }

        // Then
        Assert.True(File.Exists($"{TempDir}/{file}"), $"{file} does not exist.");
      }

      [Fact]
      public async Task VolumeAndEnvironment()
      {
        // Given
        const string target = "tmp";

        const string file = "dayOfWeek";

        var dayOfWeek = DateTime.Now.DayOfWeek.ToString();

        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx")
          .WithMount(TempDir, $"/{target}")
          .WithEnvironment("dayOfWeek", dayOfWeek)
          .WithWaitStrategy(Wait.UntilFilesExists($"{TempDir}/{file}"))
          .WithCommand("/bin/sh", "-c", $"printf $dayOfWeek > /{target}/{file}");

        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }

        // Then
        Assert.Equal(dayOfWeek, File.ReadAllText($"{TempDir}/{file}"));
      }

      [Fact]
      public async Task DockerEndpoint()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithDockerEndpoint(DockerApiEndpoint.Local.ToString())
          .WithImage("alpine");

        // When
        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }
      }

      [Fact]
      public async Task OutputConsumer()
      {
        // Given
        using (var output = new OutputConsumerConsoleFixture())
        {
          // When
          var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("nginx")
            .WithOutputConsumer(output)
            .WithCommand("/bin/sh", "-c", "hostname > /dev/stdout && hostname > /dev/stderr");

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
          .WithWaitStrategy(new WaitStrategyDelayForFiveSecondsFixture());

        // When
        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }
      }

      [Fact]
      public async Task ExecCommandInRunningContainer()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithCommand("/bin/sh", "-c", "tail -f /dev/null");

        // When
        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.Equal(255, await testcontainer.ExecAsync(new[] { "/bin/sh", "-c", "exit 255" }));
        }
      }
    }
  }
}
