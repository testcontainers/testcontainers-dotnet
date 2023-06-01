namespace Testcontainers.MariaDb;

public abstract class MariaDbContainerTest : ContainerTest<MariaDbBuilder, MariaDbContainer>
{
    protected MariaDbContainerTest(Action<MariaDbBuilder> configure = null) : base(configure)
    {
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new MySqlConnection(Container.GetConnectionString());

        // When
        connection.Open();

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "SELECT 1;";

        // When
        var execResult = await Container.ExecScriptAsync(scriptContent)
            .ConfigureAwait(false);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }

    [UsedImplicitly]
    public sealed class MariaDbUserConfiguration : MariaDbContainerTest
    {
    }

    [UsedImplicitly]
    public sealed class MariaDbRootConfiguration : MariaDbContainerTest
    {
        public MariaDbRootConfiguration()
            : base(builder => builder.WithUsername("root"))
        {
        }
    }
}