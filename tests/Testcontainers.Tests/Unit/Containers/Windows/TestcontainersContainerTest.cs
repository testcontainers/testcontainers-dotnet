namespace DotNet.Testcontainers.Tests.Unit.Containers.Windows
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public static class TestcontainersContainerTest
  {
    public sealed class WithConfiguration
    {
      [SkipOnLinuxEngine]
      public async Task UntilCommandIsCompleted()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("mcr.microsoft.com/windows/servercore:ltsc2022")
          .WithEntrypoint("PowerShell", "-NoLogo", "-Command", "ping -t localhost | Out-Null")
          .WithWaitStrategy(Wait.ForWindowsContainer()
            .UntilCommandIsCompleted("Exit !(Test-Path -Path 'C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe')"));

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.True(true);
        }
      }

      [SkipOnLinuxEngine]
      public async Task UntilPortIsAvailable()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("mcr.microsoft.com/windows/servercore:ltsc2022")
          .WithEntrypoint("PowerShell", "-NoLogo", "-Command", "$tcpListener = [System.Net.Sockets.TcpListener]1337; $tcpListener.Start(); ping -t localhost | Out-Null")
          .WithWaitStrategy(Wait.ForWindowsContainer()
            .UntilPortIsAvailable(1337));

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          Assert.True(true);
        }
      }
    }
  }
}
