namespace Testcontainers.Tests;

public abstract class PortForwardingTest : IAsyncLifetime
{
    private readonly IContainer _container;

    private PortForwardingTest(IContainer container)
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

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesHostConnection()
    {
        var exitCode = await _container.GetExitCodeAsync()
            .ConfigureAwait(true);

        var (stdout, _) = await _container.GetLogsAsync(timestampsEnabled: false)
            .ConfigureAwait(true);

        Assert.Equal(0, exitCode);
        Assert.Equal(bool.TrueString, stdout);
    }

    [UsedImplicitly]
    public sealed class PortForwardingDefaultConfiguration : PortForwardingTest, IClassFixture<HostedService>
    {
        public PortForwardingDefaultConfiguration(HostedService fixture)
            : base(new ContainerBuilder()
                .WithImage(CommonImages.Alpine)
                .WithAutoRemove(false)
                .WithEntrypoint("nc")
                .WithCommand(fixture.Host, fixture.Port.ToString(CultureInfo.InvariantCulture))
                .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()))
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class PortForwardingNetworkConfiguration : PortForwardingTest, IClassFixture<HostedService>
    {
        public PortForwardingNetworkConfiguration(HostedService fixture)
            : base(new ContainerBuilder()
                .WithImage(CommonImages.Alpine)
                .WithAutoRemove(false)
                .WithEntrypoint("nc")
                .WithCommand(fixture.Host, fixture.Port.ToString(CultureInfo.InvariantCulture))
                .WithNetwork(new NetworkBuilder().Build())
                .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()))
                .Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class HostedService : IAsyncLifetime
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly TcpListener _tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, 0));

        public HostedService()
        {
            _tcpListener.Start();
        }

        public string Host => "host.testcontainers.internal";

        public ushort Port => Convert.ToUInt16(((IPEndPoint)_tcpListener.LocalEndpoint).Port);

        public Task InitializeAsync()
        {
            return Task.WhenAny(TestcontainersSettings.ExposeHostPortsAsync(Port), AcceptSocketAsync());
        }

        public Task DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _tcpListener.Stop();

            return Task.CompletedTask;
        }

        private async Task AcceptSocketAsync()
        {
            var sendBytes = Encoding.Default.GetBytes(bool.TrueString);

            using var socket = await _tcpListener.AcceptSocketAsync(_cancellationTokenSource.Token)
                .ConfigureAwait(false);

            _ = await socket.SendAsync(sendBytes, SocketFlags.None, _cancellationTokenSource.Token)
                .ConfigureAwait(false);
        }
    }

    private sealed class WaitUntil : IWaitUntil
    {
        public Task<bool> UntilAsync(IContainer container)
        {
            return Task.FromResult(TestcontainersStates.Exited.Equals(container.State));
        }
    }
}