namespace Testcontainers.Tests;

public abstract class WaitStrategyModeTest : IAsyncLifetime
{
    private readonly IContainer _container;

    private WaitStrategyModeTest(WaitStrategyMode waitStrategyMode)
    {
        _container = new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil(), o => o.WithMode(waitStrategyMode)))
            .Build();
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _container.DisposeAsync();
    }

    public sealed class WaitStrategyModeRunningTest : WaitStrategyModeTest
    {
        public WaitStrategyModeRunningTest()
            : base(WaitStrategyMode.Running)
        {
        }

        [Fact]
        public async Task StartAsyncShouldNotSucceedWhenContainerIsNotRunning()
        {
            var exception = await Record.ExceptionAsync(() => _container.StartAsync(TestContext.Current.CancellationToken))
                .ConfigureAwait(true);

            Assert.NotNull(exception);
            Assert.IsType<ContainerNotRunningException>(exception);
        }
    }

    public sealed class WaitStrategyModeOneShotTest : WaitStrategyModeTest
    {
        public WaitStrategyModeOneShotTest()
            : base(WaitStrategyMode.OneShot)
        {
        }

        [Fact]
        public async Task StartAsyncShouldSucceedWhenContainerIsNotRunning()
        {
            var exception = await Record.ExceptionAsync(() => _container.StartAsync(TestContext.Current.CancellationToken))
                .ConfigureAwait(true);

            Assert.Null(exception);
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