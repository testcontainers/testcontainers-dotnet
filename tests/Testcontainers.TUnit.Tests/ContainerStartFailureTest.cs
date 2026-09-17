namespace Testcontainers.TUnit.Tests;

// ContainerLifetime postpones an exception thrown while starting the container and rethrows it
// when the test accesses the Container property, so the test fails with the actual cause instead
// of failing during initialization. The container examples never hit this path, so this test
// covers it explicitly.
public sealed class ContainerStartFailureTest : ContainerTest<RedisBuilder, RedisContainer>
{
    protected override RedisBuilder Configure()
    {
        // A wait strategy that never succeeds fails the container start after its timeout expires.
        return new RedisBuilder("redis:7.0")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("this message is never logged", waitStrategy => waitStrategy.WithTimeout(TimeSpan.FromSeconds(1))));
    }

    /// <summary>
    /// The container fails to start because the wait strategy times out. Accessing <c>Container</c> rethrows that exception.
    /// </summary>
    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ContainerShouldRethrowStartException()
    {
        await Assert.That(() => _ = Container).Throws<TimeoutException>();
    }
}