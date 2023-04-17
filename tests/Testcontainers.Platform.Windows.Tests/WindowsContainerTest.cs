namespace Testcontainers.Tests;

public abstract class WindowsContainerTest : IAsyncLifetime
{
    private readonly IContainer _container;

    private WindowsContainerTest(IContainer container)
    {
        _container = container;
    }

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    [SkipOnLinuxEngine]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Windows))]
    public void ContainerIsRunning()
    {
        Assert.Equal(TestcontainersStates.Running, _container.State);
    }

    [UsedImplicitly]
    public sealed class UntilCommandIsCompleted : WindowsContainerTest
    {
        public UntilCommandIsCompleted()
            : base(new ContainerBuilder()
                .WithImage(CommonImages.ServerCore)
                .WithEntrypoint("PowerShell", "-NoLogo", "-Command")
                .WithCommand("Start-Sleep -Seconds 120")
                .WithWaitStrategy(Wait.ForWindowsContainer().UntilCommandIsCompleted("Exit(-Not(Test-Path -Path 'C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe'))"))
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class UntilPortIsAvailable : WindowsContainerTest
    {
        public UntilPortIsAvailable()
            : base(new ContainerBuilder()
                .WithImage(CommonImages.ServerCore)
                .WithEntrypoint("PowerShell", "-NoLogo", "-Command")
                .WithCommand("$tcpListener = [System.Net.Sockets.TcpListener]80; $tcpListener.Start(); Start-Sleep -Seconds 120")
                .WithWaitStrategy(Wait.ForWindowsContainer().UntilPortIsAvailable(80))
                .Build())
        {
        }
    }
}