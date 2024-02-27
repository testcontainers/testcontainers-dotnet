namespace Testcontainers.Tests;

public abstract class LoggerTest : IAsyncLifetime
{
    private readonly Mock<ILogger> _mockOfILogger;

    protected LoggerTest(MockOfILogger mockOfILogger)
    {
        _mockOfILogger = mockOfILogger;
    }

    public Task InitializeAsync()
    {
        return new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithCommand(CommonCommands.SleepInfinity)
            .WithLogger(_mockOfILogger.Object)
            .Build()
            .StartAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void LogContainerRuntimeInformationOnce(int _)
    {
        Expression<Action<ILogger>> predicate = logger => logger.Log(
            It.Is<LogLevel>(logLevel => LogLevel.Information.Equals(logLevel)),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("Connected to Docker")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>());

        _mockOfILogger.Verify(predicate, Times.Once);
    }

    [UsedImplicitly]
    public sealed class SingleInstanceTest : LoggerTest
    {
        public SingleInstanceTest()
            : base(new SingleInstance())
        {
        }
    }

    [UsedImplicitly]
    public sealed class SharedInstanceTest : LoggerTest, IClassFixture<SharedInstance>
    {
        public SharedInstanceTest(SharedInstance sharedInstance)
            : base(sharedInstance)
        {
        }
    }

    [UsedImplicitly]
    [Collection(nameof(SharedCollection))]
    public sealed class SharedCollectionTest1 : LoggerTest
    {
        public SharedCollectionTest1(SharedInstance sharedInstance)
            : base(sharedInstance)
        {
        }
    }

    [UsedImplicitly]
    [Collection(nameof(SharedCollection))]
    public sealed class SharedCollectionTest2 : LoggerTest
    {
        public SharedCollectionTest2(SharedInstance sharedInstance)
            : base(sharedInstance)
        {
        }
    }

    public sealed class SingleInstance : MockOfILogger
    {
        // Ctor ITestOutputHelper
    }

    public sealed class SharedInstance : MockOfILogger
    {
        // Ctor IMessageSink
    }

    [CollectionDefinition(nameof(SharedCollection))]
    public sealed class SharedCollection : ICollectionFixture<SharedInstance>
    {
        // Ctor IMessageSink
    }

    public abstract class MockOfILogger : Mock<ILogger>
    {
        public MockOfILogger()
        {
            Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        }
    }
}