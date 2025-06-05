namespace Testcontainers.Tests;

public abstract class PortForwardingTest : IAsyncLifetime
{
    private readonly IContainer _container;

    private PortForwardingTest(IContainer container)
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

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesHostConnection()
    {
        var exitCode = await _container.GetExitCodeAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var (stdout, _) = await _container.GetLogsAsync(timestampsEnabled: false, ct: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        Assert.Equal(0, exitCode);
        Assert.Equal(bool.TrueString, stdout);
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _container.DisposeAsync();
    }

    [UsedImplicitly]
    public sealed class PortForwardingDefaultConfiguration : PortForwardingTest, IClassFixture<HostedService>
    {
        public PortForwardingDefaultConfiguration(HostedService fixture)
            : base(new ContainerBuilder()
                .WithImage(CommonImages.Alpine)
                .WithAutoRemove(false)
                .WithEntrypoint("nc")
                .WithCommand(HostedService.Host, fixture.Port.ToString(CultureInfo.InvariantCulture))
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
                .WithCommand(HostedService.Host, fixture.Port.ToString(CultureInfo.InvariantCulture))
                .WithNetwork(new NetworkBuilder().Build())
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

        public static string Host => "host.testcontainers.internal";

        public ushort Port => Convert.ToUInt16(((IPEndPoint)_tcpListener.LocalEndpoint).Port);

        public async ValueTask InitializeAsync()
        {
            await Task.WhenAny(TestcontainersSettings.ExposeHostPortsAsync(Port), AcceptSocketAsync())
                .ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _tcpListener.Stop();

            return ValueTask.CompletedTask;
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
}