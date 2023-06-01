namespace Testcontainers.Redis;

public sealed class RedisContainerTest : ContainerTest<RedisBuilder, RedisContainer>
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        using var connection = ConnectionMultiplexer.Connect(Container.GetConnectionString());
        Assert.True(connection.IsConnected);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "return 'Hello, scripting!'";

        // When
        var execResult = await Container.ExecScriptAsync(scriptContent)
            .ConfigureAwait(false);

        // When
        Assert.True("Hello, scripting!\n".Equals(execResult.Stdout), execResult.Stdout);
    }
}