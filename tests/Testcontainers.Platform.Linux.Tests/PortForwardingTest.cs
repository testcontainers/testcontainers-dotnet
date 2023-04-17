namespace Testcontainers.Tests;

[UsedImplicitly]
public sealed class PortForwardingTest : IAsyncLifetime, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private readonly TcpListener _tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, 0));

    public PortForwardingTest()
    {
        _tcpListener.Start();

        _ = Task.Run(async () =>
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var sendBytes = Encoding.Default.GetBytes(bool.TrueString);

                using var socket = await _tcpListener.AcceptSocketAsync(_cancellationTokenSource.Token)
                    .ConfigureAwait(false);

                _ = await socket.SendAsync(sendBytes, SocketFlags.None, _cancellationTokenSource.Token)
                    .ConfigureAwait(false);

                await socket.DisconnectAsync(false, _cancellationTokenSource.Token)
                    .ConfigureAwait(false);
            }
        });
    }

    private string Host => "host.testcontainers.internal";

    private ushort Port => Convert.ToUInt16(((IPEndPoint)_tcpListener.LocalEndpoint).Port);

    public Task InitializeAsync()
    {
        return TestcontainersSettings.ExposeHostPortsAsync(Port);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _tcpListener.Stop();
    }

    public sealed class MyClass : IClassFixture<PortForwardingTest>, IAsyncLifetime
    {
        private readonly IContainer _container;

        public MyClass(PortForwardingTest fixture)
        {
            _container = new ContainerBuilder()
                .WithImage(CommonImages.Alpine)
                .WithAutoRemove(false)
                .WithEntrypoint("nc")
                .WithCommand(fixture.Host, fixture.Port.ToString(CultureInfo.InvariantCulture))
                .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()))
                .Build();
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
                .ConfigureAwait(false);

            var (stdout, _) = await _container.GetLogsAsync(timestampsEnabled: false)
                .ConfigureAwait(false);

            Assert.Equal(0, exitCode);
            Assert.Equal(bool.TrueString, stdout);
        }

        private sealed class WaitUntil : IWaitUntil
        {
            public Task<bool> UntilAsync(IContainer container)
            {
                return Task.FromResult(TestcontainersStates.Exited.Equals(container.State));
            }
        }
    }
}