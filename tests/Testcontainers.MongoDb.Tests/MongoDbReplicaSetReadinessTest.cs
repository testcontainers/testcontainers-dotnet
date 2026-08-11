namespace Testcontainers.MongoDb;

public sealed class MongoDbReplicaSetReadinessTest : IAsyncLifetime
{
    private const string ReadinessMessagesScriptContent = "#!/bin/sh\necho 'Waiting for connections'\necho 'Waiting for connections'\n";

    private const string ReadinessMessagesScriptFilePath = "/docker-entrypoint-initdb.d/00-readiness-messages.sh";

    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder(TestSession.GetImageFromDockerfile())
        .WithReplicaSet()
        .WithResourceMapping(Encoding.Default.GetBytes(ReadinessMessagesScriptContent), ReadinessMessagesScriptFilePath, fileMode: Unix.FileMode755)
        .Build();

    public async ValueTask InitializeAsync()
    {
        await _mongoDbContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _mongoDbContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task StartsWithAdditionalReadinessMessages()
    {
        // Given
        const string scriptContent = "rs.status().ok;";

        // When
        var execResult = await _mongoDbContainer.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
}