namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.IO;
  using System.Net;
  using System.Net.Http;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class TestcontainersContainerTest
  {
    private static readonly string TempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));

    static TestcontainersContainerTest()
    {
      _ = Directory.CreateDirectory(TempPath);
    }

    public sealed class WithConfiguration
    {
      [Fact]
      public void ShouldThrowArgumentNullExceptionWhenBuildConfigurationHasNoImage()
      {
        Assert.Throws<ArgumentException>(() => _ = new TestcontainersBuilder<TestcontainersContainer>().Build());
      }

      [Fact]
      public async Task GeneratedContainerName()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(CommonCommands.SleepInfinity);

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
        var name = Guid.NewGuid().ToString("D");

        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(CommonCommands.SleepInfinity)
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
          .WithHostname(hostname);

        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.Equal(0, await testcontainer.GetExitCode());
        }
      }

      [Fact]
      public async Task MacAddress()
      {
        // Given
        const string macAddress = "92:95:5e:30:fe:6d";

        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .WithMacAddress(macAddress);

        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.Equal(macAddress, testcontainer.MacAddress);
        }
      }

      [Fact]
      public async Task WorkingDirectory()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint("/bin/sh", "-c", "test -d /tmp && exit $? || exit $?")
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
          .WithEntrypoint("/bin/sh", "-c", "exit 255");

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
          .WithEntrypoint(CommonCommands.SleepInfinity)
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
          .WithEntrypoint(CommonCommands.SleepInfinity)
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
          .WithBindMount(TempPath, $"/{target}")
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilFileExists(Path.Combine(TempPath, file)));

        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }

        // Then
        Assert.True(File.Exists(Path.Combine(TempPath, file)), $"{file} does not exist.");
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
          .WithNetworkAliases("Foo")
          .WithEntrypoint("/bin/sh", "-c", $"printf $dayOfWeek > /{target}/{file} && tail -f /dev/null")
          .WithEnvironment("dayOfWeek", dayOfWeek)
          .WithBindMount(TempPath, $"/{target}")
          .ConfigureContainer(_ => { }) // https://github.com/testcontainers/testcontainers-dotnet/issues/507.
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilFileExists(Path.Combine(TempPath, file)));

        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }

        // Then
        Assert.Equal(dayOfWeek, await File.ReadAllTextAsync(Path.Combine(TempPath, file)));
      }

      [Fact]
      public async Task DockerEndpoint()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithDockerEndpoint(TestcontainersSettings.OS.DockerEndpointAuthConfig)
          .WithImage("alpine")
          .WithEntrypoint(CommonCommands.SleepInfinity);

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          var exception = await Record.ExceptionAsync(() => testcontainer.StartAsync());
          Assert.Null(exception);
        }
      }

      [Theory]
      [InlineData("127.0.0.1", "npipe://./pipe/docker_engine")]
      [InlineData("127.0.0.1", "unix:/var/run/docker.sock")]
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
          .WithImage("alpine")
          .WithDockerEndpoint(endpoint);

        // When
        // Then
        await using (var testcontainer = testcontainersBuilder.Build())
        {
          Assert.Equal(expectedHostname, testcontainer.Hostname);
        }
      }

      [Fact]
      public async Task WaitStrategy()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(CommonCommands.SleepInfinity)
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
          .WithEntrypoint(CommonCommands.SleepInfinity);

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
          .WithEntrypoint(CommonCommands.SleepInfinity);

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
          .WithEntrypoint(CommonCommands.SleepInfinity);

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
          .WithEntrypoint(CommonCommands.SleepInfinity);

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          await testcontainer.CopyFileAsync(dayOfWeekFilePath, Encoding.Default.GetBytes(dayOfWeek));
          Assert.Equal(0, (await testcontainer.ExecAsync(new[] { "/bin/sh", "-c", $"test \"$(cat {dayOfWeekFilePath})\" = \"{dayOfWeek}\"" })).ExitCode);
          Assert.Equal(0, (await testcontainer.ExecAsync(new[] { "/bin/sh", "-c", $"stat {dayOfWeekFilePath} | grep 0600" })).ExitCode);
        }
      }

      [Fact]
      public async Task AutoRemoveFalseShouldNotRemoveContainer()
      {
        // Given
        string testcontainerId;

        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithAutoRemove(false)
          .WithCleanUp(false)
          .WithEntrypoint(CommonCommands.SleepInfinity);

        // When
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          testcontainerId = testcontainer.Id;
        }

        // Then
        Assert.True(DockerCli.ResourceExists(DockerCli.DockerResource.Container, testcontainerId));
      }

      [Fact]
      public async Task AutoRemoveTrueShouldRemoveContainer()
      {
        // Given
        var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .WithAutoRemove(true)
          .WithCleanUp(false)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(false);

        var id = container.Id;

        await container.DisposeAsync()
          .ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromSeconds(1))
          .ConfigureAwait(false);

        // Then
        Assert.False(DockerCli.ResourceExists(DockerCli.DockerResource.Container, id));
      }

      [Fact]
      public async Task ParameterModifiers()
      {
        // Given
        var name = Guid.NewGuid().ToString("D");

        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("alpine")
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .WithCreateContainerParametersModifier(parameters => parameters.Name = "placeholder")
          .WithCreateContainerParametersModifier(parameters => parameters.Name = name);

        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.EndsWith(name, testcontainer.Name);
        }
      }

      [Fact]
      public async Task PullPolicyNever()
      {
        // Given
        // An image that actually exists but was not used/pulled previously
        // If this image is cached/pulled before, this test will fail
        const string uncachedImage = "alpine:edge";

        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage(uncachedImage)
          .WithImagePullPolicy(PullPolicy.Never);

        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await Assert.ThrowsAnyAsync<Exception>(() => testcontainer.StartAsync());
        }
      }
    }
  }
}
