namespace Testcontainers.Tests;

public abstract class WindowsContainerTest : IAsyncLifetime
{
    private readonly IContainer _container;

    private WindowsContainerTest(IContainer container)
    {
        _container = container;
    }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync().WaitAsync(TimeSpan.FromSeconds(30))
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [SkipOnLinuxEngine]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Windows))]
    public void ContainerIsRunning()
    {
        Assert.Equal(TestcontainersStates.Running, _container.State);
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _container.DisposeAsync();
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

    [UsedImplicitly]
    public sealed class UntilHostTcpPortIsAvailable : WindowsContainerTest
    {
        public UntilHostTcpPortIsAvailable()
            : base(new ContainerBuilder()
                .WithImage(CommonImages.ServerCore)
                .WithEntrypoint("PowerShell", "-NoLogo", "-Command")
                .WithCommand("$tcpListener = [System.Net.Sockets.TcpListener]80; $tcpListener.Start();$client = $tcpListener.AcceptTcpClient(); Start-Sleep -Seconds 120")
                .WithPortBinding(80, true)
                .WithWaitStrategy(Wait.ForWindowsContainer().UntilHostTcpPortAvailable(80))
                .Build())
        {
        }
    }
}