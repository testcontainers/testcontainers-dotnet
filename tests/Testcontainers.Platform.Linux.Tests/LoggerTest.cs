namespace Testcontainers.Tests;

public abstract class LoggerTest : IAsyncLifetime
{
    private readonly FakeLogger _fakeLogger;

    protected LoggerTest(FakeLogger fakeLogger)
    {
        _fakeLogger = fakeLogger;
    }

    public async ValueTask InitializeAsync()
    {
        await new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithCommand(CommonCommands.SleepInfinity)
            .WithLogger(_fakeLogger)
            .Build()
            .StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void LogContainerRuntimeInformationOnce(int _)
    {
        Assert.Contains("Connected to Docker", _fakeLogger.Collector.GetSnapshot()[0].Message);
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return ValueTask.CompletedTask;
    }

    [UsedImplicitly]
    public sealed class SingleInstanceTest : LoggerTest
    {
        public SingleInstanceTest()
            : base(new SingleInstance()) { }
    }

    [UsedImplicitly]
    public sealed class SharedInstanceTest : LoggerTest, IClassFixture<SharedInstance>
    {
        public SharedInstanceTest(SharedInstance sharedInstance)
            : base(sharedInstance) { }
    }

    [UsedImplicitly]
    [Collection(nameof(SharedCollection))]
    public sealed class SharedCollectionTest1 : LoggerTest
    {
        public SharedCollectionTest1(SharedInstance sharedInstance)
            : base(sharedInstance) { }
    }

    [UsedImplicitly]
    [Collection(nameof(SharedCollection))]
    public sealed class SharedCollectionTest2 : LoggerTest
    {
        public SharedCollectionTest2(SharedInstance sharedInstance)
            : base(sharedInstance) { }
    }

    public sealed class SingleInstance : FakeLogger
    {
        // Ctor ITestOutputHelper
    }

    public sealed class SharedInstance : FakeLogger
    {
        // Ctor IMessageSink
    }

    [CollectionDefinition(nameof(SharedCollection))]
    public sealed class SharedCollection : ICollectionFixture<SharedInstance>
    {
        // Ctor IMessageSink
    }
}
