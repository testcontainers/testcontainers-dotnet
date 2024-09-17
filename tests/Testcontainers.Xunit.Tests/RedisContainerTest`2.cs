namespace Testcontainers.Xunit.Example2;

// # --8<-- [start:ConfigureRedisContainer]
[UsedImplicitly]
public sealed class RedisContainerFixture(IMessageSink messageSink)
    : ContainerFixture<RedisBuilder, RedisContainer>(messageSink)
{
    protected override RedisBuilder Configure(RedisBuilder builder)
    {
        return builder.WithImage(RedisBuilder.RedisImage);
    }
}
// # --8<-- [end:ConfigureRedisContainer]

// # --8<-- [start:InjectContainerFixture]
public sealed partial class RedisContainerTest(RedisContainerFixture fixture)
    : IClassFixture<RedisContainerFixture>;
// # --8<-- [end:InjectContainerFixture]

public sealed partial class RedisContainerTest : ITestCaseOrderer
{
    [Fact]
    public async Task Test1()
    {
        using var redis = await ConnectionMultiplexer.ConnectAsync(fixture.Container.GetConnectionString());
        await redis.GetDatabase().StringSetAsync("key", "value");
        Assert.True(redis.IsConnected);
    }

    [Fact]
    public async Task Test2()
    {
        using var redis = await ConnectionMultiplexer.ConnectAsync(fixture.Container.GetConnectionString());
        var redisValue = await redis.GetDatabase().StringGetAsync("key");
        // # --8<-- [start:RunTests]
        Assert.Equal("value", redisValue);
        // # --8<-- [end:RunTests]
    }

    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        return testCases.OrderBy(testCase => testCase.TestMethod.Method.Name);
    }
}