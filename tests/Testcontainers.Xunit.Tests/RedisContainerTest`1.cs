namespace Testcontainers.Xunit.Example1;

// # --8<-- [start:ConfigureRedisContainer]
public sealed partial class RedisContainerTest(ITestOutputHelper testOutputHelper)
    : ContainerTest<RedisBuilder, RedisContainer>(testOutputHelper)
{
    protected override RedisBuilder Configure()
    {
        // 👇 Configure your container instance here.
        return new RedisBuilder("redis:7.0");
    }
}
// # --8<-- [end:ConfigureRedisContainer]

#if XUNIT_V3
[TestCaseOrderer(ordererType: typeof(Testcontainers.Xunit.Tests.AlphabeticalTestCaseOrderer))]
#else
[TestCaseOrderer(ordererTypeName: "Testcontainers.Xunit.Tests.AlphabeticalTestCaseOrderer", ordererAssemblyName: "Testcontainers.Xunit.Tests")]
#endif
public sealed partial class RedisContainerTest
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ImageShouldMatchDefaultModuleImage()
    {
        Assert.Equal(RedisBuilder.RedisImage, Container.Image.FullName);
    }

    // # --8<-- [start:RunTests]
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task Test1()
    {
        // 👆 A new container instance is created and started before this method (test) runs.
        using var redis = await ConnectionMultiplexer.ConnectAsync(Container.GetConnectionString());
        await redis.GetDatabase().StringSetAsync("key", "value");
        Assert.True(redis.IsConnected);
        // 👇 The created and started container is disposed of after this method (test) completes.
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task Test2()
    {
        // 👆 A new container instance is created and started before this method (test) runs.
        using var redis = await ConnectionMultiplexer.ConnectAsync(Container.GetConnectionString());
        var redisValue = await redis.GetDatabase().StringGetAsync("key");
        Assert.True(redisValue.IsNull);
        // 👇 The created and started container is disposed of after this method (test) completes.
    }
    // # --8<-- [end:RunTests]
}