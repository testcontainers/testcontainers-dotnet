namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.Globalization;
  using System.IO;
  using System.Net;
  using System.Net.Http;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class TestcontainersContainerTest
  {
    private static readonly string TempDir = Environment.GetEnvironmentVariable("AGENT_TEMPDIRECTORY") ?? "."; // We cannot use `Path.GetTempPath()` on macOS, see: https://github.com/common-workflow-language/cwltool/issues/328.

    [Collection(nameof(Testcontainers))]
    public sealed class WithConfiguration
    {
      [Fact]
      public async Task IsLinuxEngineEnabled()
      {
        var client = new TestcontainersClient();
        Assert.False(await client.GetIsWindowsEngineEnabled());
      }

      [Fact]
      public async Task UnsafeDisposable()
      {
        // Given
        ITestcontainersContainer testcontainer = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command)
          .Build();

        // When
        await testcontainer.StartAsync();
        await testcontainer.StopAsync();
        await testcontainer.DisposeAsync();

        // Then
        Assert.Throws<InvalidOperationException>(() => testcontainer.Id);
      }

      [Fact]
      public async Task SafeDisposable()
      {
        // Given
        ITestcontainersContainer testcontainer = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command)
          .Build();

        // When
        await using (testcontainer)
        {
          await testcontainer.StartAsync();
        }

        // Then
        Assert.Throws<InvalidOperationException>(() => testcontainer.Id);
      }

      [Fact]
      public async Task GeneratedContainerName()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command);

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.NotEmpty(testcontainer.Name);
        }
      }

      [Fact]
      public async Task SpecifiedContainerName()
      {
        // Given
        var name = Guid.NewGuid().ToString("N");

        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command)
          .WithName(name);

        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.EndsWith(name, testcontainer.Name);
        }
      }

      [Fact]
      public async Task Hostname()
      {
        // Given
        const string hostname = "alpine";

        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint("/bin/sh", "-c", $"hostname | grep '{hostname}' &> /dev/null")
          .WithCleanUp(false)
          .WithHostname(hostname);

        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.Equal(0, await testcontainer.GetExitCode());
        }
      }

      [Fact]
      public async Task WorkingDirectory()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint("/bin/sh", "-c", "test -d /tmp && exit $? || exit $?")
          .WithCleanUp(false)
          .WithWorkingDirectory("/tmp");

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
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
          .WithEntrypoint("/bin/sh", "-c", "exit 255")
          .WithCleanUp(false);

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
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
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command)
          .WithExposedPort(80);

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          var exception = await Record.ExceptionAsync(() => testcontainer.StartAsync());
          Assert.Null(exception);
        }
      }

      [Theory]
      [InlineData(80, 80)]
      [InlineData(443, 80)]
      public async Task PortBindingsHttpAndHttps(int hostPort, int containerPort)
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx")
          .WithPortBinding(hostPort, containerPort)
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilPortIsAvailable(containerPort));

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();

          using (var httpClient = new HttpClient())
          {
            using (var response = await httpClient.GetAsync($"http://localhost:{hostPort}"))
            {
              Assert.True(HttpStatusCode.OK.Equals(response.StatusCode), $"nginx port {hostPort} is not available.");
            }
          }
        }
      }

      [Fact]
      public async Task RandomHostPortBindings()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx")
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command)
          .WithPortBinding(80, true);

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.NotEqual(0, testcontainer.GetMappedPublicPort(80));
        }
      }

      [Fact]
      public async Task BindMountAndCommand()
      {
        // Given
        const string target = "tmp";

        const string file = "hostname";

        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx")
          .WithEntrypoint("/bin/sh", "-c")
          .WithCommand($"hostname > /{target}/{file} && tail -f /dev/null")
          .WithBindMount(TempDir, $"/{target}")
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilFileExists(Path.Combine(TempDir, file)));

        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }

        // Then
        Assert.True(File.Exists(Path.Combine(TempDir, file)), $"{file} does not exist.");
      }

      [Fact]
      public async Task BindMountAndEnvironment()
      {
        // Given
        const string target = "tmp";

        const string file = "dayOfWeek";

        var dayOfWeek = DateTime.UtcNow.DayOfWeek.ToString();

        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx")
          .WithEntrypoint("/bin/sh", "-c", $"printf $dayOfWeek > /{target}/{file} && tail -f /dev/null")
          .WithEnvironment("dayOfWeek", dayOfWeek)
          .WithBindMount(TempDir, $"/{target}")
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilFileExists(Path.Combine(TempDir, file)));

        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }

        // Then
        Assert.Equal(dayOfWeek, await File.ReadAllTextAsync(Path.Combine(TempDir, file)));
      }

      [Fact]
      public async Task DockerEndpoint()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithDockerEndpoint(TestcontainersSettings.OS.DockerApiEndpoint.ToString())
          .WithImage("alpine")
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command);

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          var exception = await Record.ExceptionAsync(() => testcontainer.StartAsync());
          Assert.Null(exception);
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
        // Given
        var unixTimeInMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture);

        using (var consumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream()))
        {
          // When
          var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("alpine")
            .WithEntrypoint("/bin/sh", "-c", $"printf \"{unixTimeInMilliseconds}\" | tee /dev/stderr && tail -f /dev/null")
            .WithOutputConsumer(consumer)
            .WithWaitStrategy(Wait.ForUnixContainer()
              .UntilMessageIsLogged(consumer.Stdout, unixTimeInMilliseconds)
              .UntilMessageIsLogged(consumer.Stderr, unixTimeInMilliseconds));

          await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
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
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command)
          .WithWaitStrategy(Wait.ForUnixContainer()
            .AddCustomWaitStrategy(new WaitUntilFiveSecondsPassedFixture()));

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          var exception = await Record.ExceptionAsync(() => testcontainer.StartAsync());
          Assert.Null(exception);
        }
      }

      [Fact]
      public async Task ExecCommandInRunningContainer()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command);

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          var execResult = await testcontainer.ExecAsync(new[] { "/bin/sh", "-c", "exit 255" });
          Assert.Equal(255, execResult.ExitCode);
        }
      }

      [Fact]
      public async Task ExecCommandInRunningContainerWithStdout()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command);

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          var execResult = await testcontainer.ExecAsync(new[] { "/bin/sh", "-c", "ping -c 4 google.com" });
          Assert.Contains("PING google.com", execResult.Stdout);
        }
      }

      [Fact]
      public async Task ExecCommandInRunningContainerWithStderr()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command);

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          var execResult = await testcontainer.ExecAsync(new[] { "/bin/sh", "-c", "cd missing_directory" });
          Assert.Contains("can't cd to missing_directory", execResult.Stderr);
        }
      }

      [Fact]
      public async Task CopyFileToRunningContainer()
      {
        // Given
        const string dayOfWeekFilePath = "/tmp/dayOfWeek";

        var dayOfWeek = DateTime.UtcNow.DayOfWeek.ToString();

        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(KeepTestcontainersUpAndRunning.Command);

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          await testcontainer.CopyFileAsync(dayOfWeekFilePath, Encoding.UTF8.GetBytes(dayOfWeek));
          Assert.Equal(0, (await testcontainer.ExecAsync(new[] { "/bin/sh", "-c", $"test \"$(cat {dayOfWeekFilePath})\" = \"{dayOfWeek}\"" })).ExitCode);
          Assert.Equal(0, (await testcontainer.ExecAsync(new[] { "/bin/sh", "-c", $"stat {dayOfWeekFilePath} | grep 0600" })).ExitCode);
        }
      }
    }
  }
}
