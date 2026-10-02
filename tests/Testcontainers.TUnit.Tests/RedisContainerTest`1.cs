namespace Testcontainers.TUnit.Example1;

// # --8<-- [start:ConfigureRedisContainer]
public sealed partial class RedisContainerTest : ContainerTest<RedisBuilder, RedisContainer>
{
    protected override RedisBuilder Configure()
    {
        // 👇 Configure your container instance here.
        return new RedisBuilder("redis:7.0");
    }
}
// # --8<-- [end:ConfigureRedisContainer]

public sealed partial class RedisContainerTest
{
    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ImageShouldMatchDefaultModuleImage()
    {
        await Assert.That(Container.Image.FullName).IsEqualTo(RedisBuilder.RedisImage);
    }

    // # --8<-- [start:RunTests]
    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task Test1()
    {
        // 👆 A new container instance is created and started before this method (test) runs.
        using var redis = await ConnectionMultiplexer.ConnectAsync(Container.GetConnectionString());
        await redis.GetDatabase().StringSetAsync("key", "value");
        await Assert.That(redis.IsConnected).IsTrue();
        // 👇 The created and started container is disposed of after this method (test) completes.
    }

    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task Test2()
    {
        // 👆 A new container instance is created and started before this method (test) runs.
        using var redis = await ConnectionMultiplexer.ConnectAsync(Container.GetConnectionString());
        var redisValue = await redis.GetDatabase().StringGetAsync("key");
        await Assert.That(redisValue.IsNull).IsTrue();
        // 👇 The created and started container is disposed of after this method (test) completes.
    }
    // # --8<-- [end:RunTests]
}