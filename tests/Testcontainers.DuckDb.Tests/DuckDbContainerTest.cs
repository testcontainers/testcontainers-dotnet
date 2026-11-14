namespace Testcontainers.DuckDb;

public sealed class DuckDbContainerTest(DuckDbContainerTest.DuckDbFixture fixture)
    : IClassFixture<DuckDbContainerTest.DuckDbFixture>
{
    /// <summary>
    /// Verifies that executing a SQL script returns a successful exit code.
    /// </summary>
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

    /// <summary>
    /// Verifies that the database file retains state across script executions.
    /// </summary>
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

    /// <summary>
    /// Verifies that concurrent script executions are serialized and do not fail with
    /// a database-lock error (DuckDB does not support concurrent write access to the
    /// same database file from multiple processes).
    /// </summary>
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ConcurrentScriptExecutionsReturnSuccessful()
    {
        // Given
        const int numberOfExecutions = 5;

        const string createScript = "CREATE TABLE IF NOT EXISTS measurements (id INTEGER);";

        // When
        var createResult = await fixture.Container.ExecScriptAsync(createScript, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var execResults = await Task.WhenAll(Enumerable.Range(0, numberOfExecutions)
                .Select(id => fixture.Container.ExecScriptAsync($"INSERT INTO measurements VALUES ({id});", TestContext.Current.CancellationToken)))
            .ConfigureAwait(true);

        var countResult = await fixture.Container.ExecScriptAsync("SELECT count(*) FROM measurements;", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(createResult.ExitCode), createResult.Stderr);
        Assert.All(execResults, execResult => Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr));
        Assert.True(0L.Equals(countResult.ExitCode), countResult.Stderr);
        Assert.Contains(numberOfExecutions.ToString(), countResult.Stdout);
    }

    /// <summary>
    /// Verifies that non-ASCII characters in SQL scripts survive the UTF-8 round trip.
    /// </summary>
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptWithNonAsciiCharactersReturnsSuccessful()
    {
        // Given
        const string scriptContent = "SELECT 'héllo wörld' AS greeting;";

        // When
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Contains("héllo wörld", execResult.Stdout);
    }

    /// <summary>
    /// Verifies that the container returns the configured database file path.
    /// </summary>
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetDatabaseFilePathReturnsConfiguredDatabase()
    {
        Assert.Equal(DuckDbBuilder.DefaultDatabaseFilePath, fixture.Container.GetDatabaseFilePath());
    }

    /// <summary>
    /// Fixture that shares a single DuckDB container instance across the tests.
    /// </summary>
    [UsedImplicitly]
    public class DuckDbFixture(IMessageSink messageSink)
        : ContainerFixture<DuckDbBuilder, DuckDbContainer>(messageSink)
    {
        /// <inheritdoc />
        protected override DuckDbBuilder Configure()
            => new DuckDbBuilder(TestSession.GetImageFromDockerfile());
    }
}
