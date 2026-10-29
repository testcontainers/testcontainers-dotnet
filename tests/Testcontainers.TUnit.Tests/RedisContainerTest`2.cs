namespace Testcontainers.TUnit.Example2;

// # --8<-- [start:ConfigureRedisContainer]
[UsedImplicitly]
public sealed class RedisContainerFixture : ContainerFixture<RedisBuilder, RedisContainer>
{
    protected override RedisBuilder Configure()
    {
        return new RedisBuilder("redis:7.0");
    }
}
// # --8<-- [end:ConfigureRedisContainer]

// # --8<-- [start:InjectContainerFixture]
[ClassDataSource<RedisContainerFixture>(Shared = SharedType.PerClass)]
public sealed partial class RedisContainerTest(RedisContainerFixture fixture);
// # --8<-- [end:InjectContainerFixture]

public sealed partial class RedisContainerTest
{
    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ImageShouldMatchDefaultModuleImage()
    {
        await Assert.That(fixture.Container.Image.FullName).IsEqualTo(RedisBuilder.RedisImage);
    }

    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task Test1()
    {
        using var redis = await ConnectionMultiplexer.ConnectAsync(fixture.Container.GetConnectionString());
        await redis.GetDatabase().StringSetAsync("key", "value");
        await Assert.That(redis.IsConnected).IsTrue();
    }

    // # --8<-- [start:RunTests]
    [Test]
    [DependsOn(nameof(Test1))]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task Test2()
    {
        using var redis = await ConnectionMultiplexer.ConnectAsync(fixture.Container.GetConnectionString());
        var redisValue = await redis.GetDatabase().StringGetAsync("key");
        await Assert.That(redisValue.ToString()).IsEqualTo("value");
    }
    // # --8<-- [end:RunTests]
}