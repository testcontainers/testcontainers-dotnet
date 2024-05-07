namespace Testcontainers.Tests;

public sealed class WaitStrategyTest
{
    [Fact]
    public Task WithTimeout()
    {
        return Assert.ThrowsAsync<TimeoutException>(() => new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithEntrypoint(CommonCommands.SleepInfinity)
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(FailingWaitStrategy.Instance, o => o.WithTimeout(TimeSpan.FromSeconds(1))))
            .Build()
            .StartAsync());
    }

    [Fact]
    public Task WithRetries()
    {
        return Assert.ThrowsAsync<RetryLimitExceededException>(() => new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithEntrypoint(CommonCommands.SleepInfinity)
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(FailingWaitStrategy.Instance, o => o.WithRetries(1)))
            .Build()
            .StartAsync());
    }

    private sealed class FailingWaitStrategy : IWaitUntil
    {
        static FailingWaitStrategy()
        {
        }

        private FailingWaitStrategy()
        {
        }

        public static IWaitUntil Instance { get; }
            = new FailingWaitStrategy();

        public Task<bool> UntilAsync(IContainer container)
        {
            return Task.FromResult(false);
        }
    }
}