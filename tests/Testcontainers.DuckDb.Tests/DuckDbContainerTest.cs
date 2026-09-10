namespace Testcontainers.DuckDb;

public sealed class DuckDbContainerTest(DuckDbContainerTest.DuckDbFixture fixture)
    : IClassFixture<DuckDbContainerTest.DuckDbFixture>
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "SELECT 1;";

        // When
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task DatabaseStatePersistsAcrossScriptExecutions()
    {
        // Given
        const string createScript = "CREATE TABLE IF NOT EXISTS persons (id INTEGER, name VARCHAR); INSERT INTO persons VALUES (1, 'duckdb');";

        const string selectScript = "SELECT name FROM persons WHERE id = 1;";

        // When
        var createResult = await fixture.Container.ExecScriptAsync(createScript, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var selectResult = await fixture.Container.ExecScriptAsync(selectScript, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(createResult.ExitCode), createResult.Stderr);
        Assert.True(0L.Equals(selectResult.ExitCode), selectResult.Stderr);
        Assert.Contains("duckdb", selectResult.Stdout);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetDatabaseFilePathReturnsConfiguredDatabase()
    {
        Assert.Equal(DuckDbBuilder.DefaultDatabaseFilePath, fixture.Container.GetDatabaseFilePath());
    }

    [UsedImplicitly]
    public class DuckDbFixture(IMessageSink messageSink)
        : ContainerFixture<DuckDbBuilder, DuckDbContainer>(messageSink)
    {
        protected override DuckDbBuilder Configure()
            => new DuckDbBuilder(TestSession.GetImageFromDockerfile());
    }
}
