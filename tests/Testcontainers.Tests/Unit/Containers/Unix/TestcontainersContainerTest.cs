namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Globalization;
  using System.IO;
  using System.Net;
  using System.Net.Sockets;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class TestcontainersContainerTest
  {
    public sealed class WithConfiguration
    {
      [Fact]
      public void ShouldThrowArgumentNullExceptionWhenBuildConfigurationHasNoImage()
      {
        Assert.Throws<ArgumentException>(() => _ = new ContainerBuilder().Build());
      }

      [Fact]
      public async Task GeneratedContainerName()
      {
        // Given
        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        // Then
        Assert.NotEmpty(container.Name);
      }

      [Fact]
      public async Task SpecifiedContainerName()
      {
        // Given
        var name = Guid.NewGuid().ToString("D");

        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .WithName(name)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        // Then
        Assert.EndsWith(name, container.Name);
      }

      [Fact]
      public async Task Hostname()
      {
        // Given
        var hostname = Guid.NewGuid().ToString("D");

        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint("/bin/sh", "-c", $"hostname | grep '{hostname}' &> /dev/null")
          .WithHostname(hostname)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        var exitCode = await container.GetExitCodeAsync()
          .ConfigureAwait(true);

        // Then
        Assert.Equal(0, exitCode);
      }

      [Fact]
      public async Task MacAddress()
      {
        // Given
        const string macAddress = "92:95:5e:30:fe:6d";

        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .WithMacAddress(macAddress)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        // Then
        Assert.Equal(macAddress, container.MacAddress);
      }

      [Fact]
      public async Task WorkingDirectory()
      {
        // Given
        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint("/bin/sh", "-c", "test -d /tmp && exit $? || exit $?")
          .WithWorkingDirectory("/tmp")
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        var exitCode = await container.GetExitCodeAsync()
          .ConfigureAwait(true);

        // Then
        Assert.Equal(0, exitCode);
      }

      [Fact]
      public async Task Entrypoint()
      {
        // Given
        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint("/bin/sh", "-c", "exit 255")
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        var exitCode = await container.GetExitCodeAsync()
          .ConfigureAwait(true);

        // Then
        Assert.Equal(255, exitCode);
      }

      [Fact]
      public async Task StaticPortBinding()
      {
        // Given
        const ushort containerPort = 80;

        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        var hostPort = ((IPEndPoint)socket.LocalEndPoint).Port;

        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Nginx)
          .WithPortBinding(hostPort, containerPort)
          .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
            request.ForPort(containerPort)))
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        // Then
        Assert.Equal(hostPort, container.GetMappedPublicPort(containerPort));
      }

      [Fact]
      public async Task RandomPortBinding()
      {
        // Given
        const ushort containerPort = 80;

        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Nginx)
          .WithPortBinding(containerPort, true)
          .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
            request.ForPort(containerPort)))
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        // Then
        Assert.NotEqual(containerPort, container.GetMappedPublicPort(containerPort));
      }

      [Fact]
      public async Task UnboundPortBindingThrowsException()
      {
        // Given
        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .WithPortBinding(80, true)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        // Then
        Assert.Throws<InvalidOperationException>(() => container.GetMappedPublicPort(443));
      }

      [Fact]
      public async Task BindMountAndCommand()
      {
        // Given
        const string target = "tmp";

        const string file = "hostname";

        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Nginx)
          .WithEntrypoint("/bin/sh", "-c")
          .WithCommand($"hostname > /{target}/{file}")
          .WithBindMount(TestSession.TempDirectoryPath, $"/{target}")
          .WithWaitStrategy(Wait.ForUnixContainer().UntilFileExists(Path.Combine(TestSession.TempDirectoryPath, file)))
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        // Then
        var fileInfo = new FileInfo(Path.Combine(TestSession.TempDirectoryPath, file));
        Assert.True(fileInfo.Exists);
        Assert.True(fileInfo.Length > 0);
      }

      [Fact]
      public async Task BindMountAndEnvironment()
      {
        // Given
        const string target = "tmp";

        const string file = "dayOfWeek";

        var dayOfWeek = DateTime.UtcNow.DayOfWeek.ToString();

        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Nginx)
          .WithEnvironment("DAY_OF_WEEK", dayOfWeek)
          .WithEntrypoint("/bin/sh", "-c")
          .WithCommand($"printf $DAY_OF_WEEK > /{target}/{file}")
          .WithBindMount(TestSession.TempDirectoryPath, $"/{target}")
          .WithWaitStrategy(Wait.ForUnixContainer().UntilFileExists(Path.Combine(TestSession.TempDirectoryPath, file)))
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        // Then
        var fileInfo = new FileInfo(Path.Combine(TestSession.TempDirectoryPath, file));
        Assert.True(fileInfo.Exists);
        Assert.True(fileInfo.Length > 0);
      }

      [Fact]
      public async Task DockerEndpoint()
      {
        // Given
        await using var container = new ContainerBuilder()
          .WithDockerEndpoint(TestcontainersSettings.OS.DockerEndpointAuthConfig)
          .WithImage(CommonImages.Alpine)
          .Build();

        // When
        var exception = await Record.ExceptionAsync(() => container.StartAsync())
          .ConfigureAwait(true);

        // Then
        Assert.Null(exception);
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
        await using var container = new ContainerBuilder()
          .WithDockerEndpoint(endpoint)
          .WithImage(CommonImages.Alpine)
          .Build();

        Assert.Equal(expectedHostname, container.Hostname);
      }

      [Fact]
      public async Task OutputConsumer()
      {
        // Given
        using var consumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());

        var unixTimeInMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture);

        // When
        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint("/bin/sh", "-c")
          .WithCommand($"printf \"%s\" \"{unixTimeInMilliseconds}\" | tee /dev/stderr")
          .WithOutputConsumer(consumer)
          .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(unixTimeInMilliseconds))
          .Build();

        await container.StartAsync()
          .ConfigureAwait(true);

        consumer.Stdout.Seek(0, SeekOrigin.Begin);
        consumer.Stderr.Seek(0, SeekOrigin.Begin);

        using var stdoutReader = new StreamReader(consumer.Stdout, leaveOpen: true);
        var stdout = await stdoutReader.ReadToEndAsync()
          .ConfigureAwait(true);

        using var stderrReader = new StreamReader(consumer.Stderr, leaveOpen: true);
        var stderr = await stderrReader.ReadToEndAsync()
          .ConfigureAwait(true);

        // Then
        Assert.Equal(unixTimeInMilliseconds, stdout);
        Assert.Equal(unixTimeInMilliseconds, stderr);
      }

      [Fact]
      public async Task WaitStrategy()
      {
        // Given
        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntilFiveSecondsPassedFixture()))
          .Build();

        // When
        var exception = await Record.ExceptionAsync(() => container.StartAsync())
          .ConfigureAwait(true);

        // Then
        Assert.Null(exception);
      }

      [Fact]
      public async Task ExecCommandInRunningContainer()
      {
        // Given
        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        var execResult = await container.ExecAsync(new[] { "/bin/sh", "-c", "exit 255" })
          .ConfigureAwait(true);

        // Then
        Assert.Equal(255, execResult.ExitCode);
      }

      [Fact]
      public async Task ExecCommandInRunningContainerWithStdout()
      {
        // Given
        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        var execResult = await container.ExecAsync(new[] { "ping", "-c", "1", "google.com" })
          .ConfigureAwait(true);

        // Then
        Assert.Contains("PING google.com", execResult.Stdout);
      }

      [Fact]
      public async Task ExecCommandInRunningContainerWithStderr()
      {
        // Given
        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        var execResult = await container.ExecAsync(new[] { "ping", "-c", "1", "google.invalid" })
          .ConfigureAwait(true);

        // Then
        Assert.Contains("ping: bad address 'google.invalid'", execResult.Stderr);
      }

      [Fact]
      public async Task CopyFileToRunningContainer()
      {
        // Given
        const string dayOfWeekFilePath = "/tmp/dayOfWeek";

        var dayOfWeek = DateTime.UtcNow.DayOfWeek.ToString();

        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        await container.CopyAsync(Encoding.Default.GetBytes(dayOfWeek), dayOfWeekFilePath)
          .ConfigureAwait(true);

        var execResult = await container.ExecAsync(new[] { "test", "-f", dayOfWeekFilePath })
          .ConfigureAwait(true);

        // Then
        Assert.Equal(0, execResult.ExitCode);
      }

      [Fact]
      public async Task AutoRemoveFalseShouldNotRemoveContainer()
      {
        // Given
        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .WithAutoRemove(false)
          .WithCleanUp(false)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        var id = container.Id;

        await Task.Delay(TimeSpan.FromSeconds(1))
          .ConfigureAwait(true);

        // Then
        Assert.True(DockerCli.ResourceExists(DockerCli.DockerResource.Container, id));
      }

      [Fact]
      public async Task AutoRemoveTrueShouldRemoveContainer()
      {
        // Given
        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .WithAutoRemove(true)
          .WithCleanUp(false)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        var id = container.Id;

        await container.StopAsync()
          .ConfigureAwait(true);

        await Task.Delay(TimeSpan.FromSeconds(1))
          .ConfigureAwait(true);

        // Then
        Assert.False(DockerCli.ResourceExists(DockerCli.DockerResource.Container, id));
      }

      [Fact]
      public async Task ParameterModifiers()
      {
        // Given
        var name = Guid.NewGuid().ToString("D");

        await using var container = new ContainerBuilder()
          .WithImage(CommonImages.Alpine)
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .WithCreateParameterModifier(parameterModifier => parameterModifier.Name = "placeholder")
          .WithCreateParameterModifier(parameterModifier => parameterModifier.Name = name)
          .Build();

        // When
        await container.StartAsync()
          .ConfigureAwait(true);

        // Then
        Assert.EndsWith(name, container.Name);
      }

      [Fact]
      public async Task PullPolicyNever()
      {
        await using var container = new ContainerBuilder()
          .WithImage("alpine:3")
          .WithEntrypoint(CommonCommands.SleepInfinity)
          .WithImagePullPolicy(PullPolicy.Never)
          .Build();

        await Assert.ThrowsAnyAsync<Exception>(() => container.StartAsync())
          .ConfigureAwait(true);
      }
    }
  }
}
