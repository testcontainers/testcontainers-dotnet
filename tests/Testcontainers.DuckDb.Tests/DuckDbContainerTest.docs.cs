namespace Testcontainers.DuckDb;

// <!-- -8<- [start:UseDuckDbContainer] -->
public sealed class DuckDbContainerExample : IAsyncLifetime
{
    private readonly DuckDbContainer _duckDbContainer = new DuckDbBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _duckDbContainer.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _duckDbContainer.DisposeAsync()
            .ConfigureAwait(false);
    }
    // <!-- -8<- [end:UseDuckDbContainer] -->

    // <!-- -8<- [start:RunSQLScript] -->
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "SELECT 1;";

        // When
        var execResult = await _duckDbContainer.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
    // <!-- -8<- [end:RunSQLScript] -->
}
