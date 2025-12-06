namespace Testcontainers.Tests;

public abstract class PortBindingTest : IAsyncLifetime
{
    private readonly IContainer _container;

    private PortBindingTest(ushort[] expectedPorts)
    {
        var containerBuilder = new ContainerBuilder(CommonImages.Alpine).WithEntrypoint(CommonCommands.SleepInfinity);
        _container = expectedPorts.Aggregate(containerBuilder, (builder, port) => builder.WithPortBinding(port, true)).Build();
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

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _container.DisposeAsync();
    }

    public abstract class MappedPublicPorts : PortBindingTest
    {
        private readonly ushort[] _expectedPorts;

        protected MappedPublicPorts(ushort[] expectedPorts)
            : base(expectedPorts)
        {
            _expectedPorts = expectedPorts;
        }

        [Fact]
        public void ShouldReturnExpectedPorts()
        {
            var actualPorts = _container.GetMappedPublicPorts();
            Assert.Equal(_expectedPorts, actualPorts.Keys);
        }

        [Fact]
        public void ShouldThrowWhenNoPortsExist()
        {
            var exception = Record.Exception(() => _container.GetMappedPublicPort());
            Assert.Equal(exception == null, _expectedPorts.Length > 0);
        }
    }

    public sealed class NoPortBindingTest()
        : MappedPublicPorts(Array.Empty<ushort>());

    public sealed class SinglePortBindingTest()
        : MappedPublicPorts(new ushort[] { 8080 });

    public sealed class MultiplePortBindingTest()
        : MappedPublicPorts(new ushort[] { 8080, 8081 });
}