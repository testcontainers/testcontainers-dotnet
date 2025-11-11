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
        await _container.StartAsync()
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
    public sealed class UntilInternalTcpPortIsAvailable : WindowsContainerTest
    {
        public UntilInternalTcpPortIsAvailable()
            : base(new ContainerBuilder()
                .WithImage(CommonImages.ServerCore)
                .WithEntrypoint("PowerShell", "-NoLogo", "-Command")
                .WithCommand("$tcpListener = [System.Net.Sockets.TcpListener]::new([System.Net.IPAddress]::Any, 8080); $tcpListener.Start(); Start-Sleep -Seconds 120")
                .WithWaitStrategy(Wait.ForWindowsContainer().UntilInternalTcpPortIsAvailable(8080))
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class UntilExternalTcpPortIsAvailable : WindowsContainerTest
    {
        public UntilExternalTcpPortIsAvailable()
            : base(new ContainerBuilder()
                .WithImage(CommonImages.ServerCore)
                .WithPortBinding(8080, true)
                .WithEntrypoint("PowerShell", "-NoLogo", "-Command")
                .WithCommand("$tcpListener = [System.Net.Sockets.TcpListener]::new([System.Net.IPAddress]::Any, 8080); $tcpListener.Start(); Start-Sleep -Seconds 120")
                .WithWaitStrategy(Wait.ForWindowsContainer().UntilExternalTcpPortIsAvailable(8080))
                .Build())
        {
        }
    }
}