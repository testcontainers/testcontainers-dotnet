namespace DotNet.Testcontainers.Tests.Unit.Containers.Windows
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public static class TestcontainersContainerTest
  {
    [Collection(nameof(Testcontainers))]
    public sealed class WithConfiguration
    {
      [SkipOnLinuxEngine]
      public async Task IsWindowsEngineEnabled()
      {
        var client = new TestcontainersClient();
        Assert.True(await client.GetIsWindowsEngineEnabled());
      }

      [SkipOnLinuxEngine]
      public async Task UntilCommandIsCompleted()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("mcr.microsoft.com/windows/servercore:1809")
          .WithEntrypoint("PowerShell", "-Command", "Start-Sleep -Seconds 120")
          .WithWaitStrategy(Wait.ForWindowsContainer()
            .UntilCommandIsCompleted("exit !(Test-Path -Path 'C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe')"));

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.Equal(0, await testcontainer.GetExitCode());
        }
      }

      [SkipOnLinuxEngine]
      public async Task UntilPortIsAvailable()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("mcr.microsoft.com/windows/servercore:1809")
          .WithEntrypoint("PowerShell", "-Command", "$tcpListener = [System.Net.Sockets.TcpListener]1337; $tcpListener.Start(); Start-Sleep -Seconds 120")
          .WithWaitStrategy(Wait.ForWindowsContainer()
            .UntilPortIsAvailable(1337));

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.Equal(0, await testcontainer.GetExitCode());
        }
      }
    }
  }
}
