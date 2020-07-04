namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.IO;
  using System.Net;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Modules;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class TestcontainersContainerTest
  {
    private static readonly string TempDir = Environment.GetEnvironmentVariable("AGENT_TEMPDIRECTORY") ?? "."; // We cannot use `Path.GetTempPath()` on macOS, see: https://github.com/common-workflow-language/cwltool/issues/328.

    public class With
    {
      [Fact]
      public async Task IsLinuxEngineEnabled()
      {
        Assert.False(await new TestcontainersClient().GetIsWindowsEngineEnabled());
      }

      [Fact]
      public async Task UnsafeDisposable()
      {
        // Given
        IDockerContainer testcontainer = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithLabel(typeof(TestcontainersContainer).Assembly.GetName().Name, nameof(this.UnsafeDisposable))
          .Build();

        // When
        // Then
        await testcontainer.StartAsync();
        await testcontainer.StopAsync();
        await testcontainer.DisposeAsync();
      }

      [Fact]
      public async Task SafeDisposable()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine");

        // When
        // Then
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
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
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
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
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
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
          .WithCommand("/bin/sh", "-c", "test -d /tmp && exit $? || exit $?")
          .WithWorkingDirectory("/tmp");

        // When
        // Then
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
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
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
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
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
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
          await using (IDockerContainer testcontainer = testcontainersBuilder
            .WithPortBinding(port.From, port.To)
            .WithWaitStrategy(Wait.ForUnixContainer()
              .UntilPortIsAvailable(port.To))
            .Build())
          {
            await testcontainer.StartAsync();

            var request = WebRequest.Create($"http://localhost:{port.From}");

            var response = (HttpWebResponse)await request.GetResponseAsync();

            Assert.True(HttpStatusCode.OK.Equals(response.StatusCode), $"nginx port {port.From} is not available.");
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
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
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
          .WithCommand("/bin/sh", "-c", $"hostname > /{target}/{file}")
          .WithMount(TempDir, $"/{target}")
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilFileExists($"{TempDir}/{file}"));

        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
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
          .WithCommand("/bin/sh", "-c", $"printf $dayOfWeek > /{target}/{file}")
          .WithEnvironment("dayOfWeek", dayOfWeek)
          .WithMount(TempDir, $"/{target}")
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilFileExists($"{TempDir}/{file}"));

        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }

        // Then
        Assert.Equal(dayOfWeek, await File.ReadAllTextAsync($"{TempDir}/{file}"));
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
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }
      }

      [Theory]
      [InlineData("localhost", "npipe://./pipe/docker_engine")]
      [InlineData("localhost", "unix:/var/run/docker.sock")]
      [InlineData("docker", "http://docker")]
      [InlineData("docker", "https://docker")]
      [InlineData("docker", "tcp://docker")]
      [InlineData("1.1.1.1", "http://1.1.1.1")]
      [InlineData("1.1.1.1", "https://1.1.1.1")]
      [InlineData("1.1.1.1", "tcp://1.1.1.1")]
      public async Task HostnameShouldMatchDockerGatewayAddress(string expectedHostname, string endpoint)
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithDockerEndpoint(endpoint);

        // When
        // Then
        await using (var testcontainer = testcontainersBuilder.Build())
        {
          Assert.Equal(expectedHostname, testcontainer.Hostname);
        }
      }

      [Fact]
      public async Task OutputConsumer()
      {
        var unixTimeInMilliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

        // Given
        using (var consumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream()))
        {
          // When
          var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("alpine")
            .WithCommand("/bin/sh", "-c", $"printf \"{unixTimeInMilliseconds}\" | tee /dev/stderr")
            .WithOutputConsumer(consumer)
            .WithWaitStrategy(Wait.ForUnixContainer()
              .UntilMessageIsLogged(consumer.Stdout, unixTimeInMilliseconds)
              .UntilMessageIsLogged(consumer.Stderr, unixTimeInMilliseconds));

          await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
          {
            await testcontainer.StartAsync();
          }

          consumer.Stdout.Seek(0, SeekOrigin.Begin);
          consumer.Stderr.Seek(0, SeekOrigin.Begin);

          // Then
          using (var streamReader = new StreamReader(consumer.Stdout, leaveOpen: true))
          {
            Assert.Equal(unixTimeInMilliseconds, await streamReader.ReadToEndAsync());
          }

          using (var streamReader = new StreamReader(consumer.Stderr, leaveOpen: true))
          {
            Assert.Equal(unixTimeInMilliseconds, await streamReader.ReadToEndAsync());
          }
        }
      }

      [Fact]
      public async Task WaitStrategy()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithWaitStrategy(Wait.ForUnixContainer()
            .AddCustomWaitStrategy(new WaitStrategyDelayForFiveSecondsFixture()));

        // When
        // Then
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
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
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.Equal(255, await testcontainer.ExecAsync(new[] { "/bin/sh", "-c", "exit 255" }));
        }
      }
    }
  }
}
