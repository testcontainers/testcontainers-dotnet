namespace Testcontainers.Xunit.Example2;

// # --8<-- [start:ConfigureRedisContainer]
[UsedImplicitly]
public sealed class RedisContainerFixture(IMessageSink messageSink)
    : ContainerFixture<RedisBuilder, RedisContainer>(messageSink)
{
    protected override RedisBuilder Configure(RedisBuilder builder)
    {
        return builder.WithImage("redis:7.0");
    }
}

// # --8<-- [end:ConfigureRedisContainer]

// # --8<-- [start:InjectContainerFixture]
public sealed partial class RedisContainerTest(RedisContainerFixture fixture)
    : IClassFixture<RedisContainerFixture>;
// # --8<-- [end:InjectContainerFixture]

#if XUNIT_V3
[TestCaseOrderer(ordererType: typeof(Testcontainers.Xunit.Tests.AlphabeticalTestCaseOrderer))]
#else
[TestCaseOrderer(
    ordererTypeName: "Testcontainers.Xunit.Tests.AlphabeticalTestCaseOrderer",
    ordererAssemblyName: "Testcontainers.Xunit.Tests"
)]
#endif
public sealed partial class RedisContainerTest
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ImageShouldMatchDefaultModuleImage()
    {
        Assert.Equal(RedisBuilder.RedisImage, fixture.Container.Image.FullName);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task Test1()
    {
        using var redis = await ConnectionMultiplexer.ConnectAsync(
            fixture.Container.GetConnectionString()
        );
        await redis.GetDatabase().StringSetAsync("key", "value");
        Assert.True(redis.IsConnected);
    }

    // # --8<-- [start:RunTests]
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task Test2()
    {
        using var redis = await ConnectionMultiplexer.ConnectAsync(
            fixture.Container.GetConnectionString()
        );
        var redisValue = await redis.GetDatabase().StringGetAsync("key");
        Assert.Equal("value", redisValue);
    }
    // # --8<-- [end:RunTests]
}
