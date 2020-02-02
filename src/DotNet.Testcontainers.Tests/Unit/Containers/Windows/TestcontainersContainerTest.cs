namespace DotNet.Testcontainers.Tests.Unit.Containers.Windows
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Modules;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using Xunit;

  public static class TestcontainersContainerTest
  {
    public class With
    {
      [IgnoreOnLinuxEngine]
      public async Task IsWindowsEngineEnabled()
      {
        Assert.True(await new TestcontainersClient().GetIsWindowsEngineEnabled());
      }

      [IgnoreOnLinuxEngine]
      public async Task UntilCommandIsCompleted()
      {
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("mcr.microsoft.com/windows/servercore:1809")
          .WithEntrypoint("PowerShell", "-Command", "Start-Sleep -Seconds 3600")
          .WithWaitStrategy(Wait.ForWindowsContainer()
            .UntilCommandIsCompleted("exit !(Test-Path -Path 'C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe')"));

        // When
        // Then
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }
      }

      [IgnoreOnLinuxEngine]
      public async Task UntilPortIsAvailable()
      {
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("mcr.microsoft.com/windows/servercore:1809")
          .WithEntrypoint("PowerShell", "-Command", "$tcpListener = [System.Net.Sockets.TcpListener]1337; $tcpListener.Start(); Start-Sleep -Seconds 3600")
          .WithWaitStrategy(Wait.ForWindowsContainer()
            .UntilPortIsAvailable(1337));

        // When
        // Then
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }
      }
    }
  }
}
