using System.Text;
using System.Threading;
using DotNet.Testcontainers.Configurations;

namespace Testcontainers.MongoDb;

/// <summary>
/// The readiness check that runs before the replica set is initiated counts occurrences of a log
/// message and compares that count for equality. Any other log line carrying the same text pushes
/// the count past the expected value, and it can then never match
/// (https://github.com/testcontainers/testcontainers-dotnet/issues/1732).
/// </summary>
public sealed class MongoDbReplicaSetReadinessTest : IAsyncLifetime
{
    private const string ExtraMarkerScriptFilePath = "/docker-entrypoint-initdb.d/00-extra-marker.sh";

    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder(TestSession.GetImageFromDockerfile())
        .WithReplicaSet()
        .WithResourceMapping(
            Encoding.Default.GetBytes("#!/bin/bash\necho 'Waiting for connections'\necho 'Waiting for connections'\n"),
            ExtraMarkerScriptFilePath,
            fileMode: Unix.FileMode755)
        .Build();

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _mongoDbContainer.DisposeAsync()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task StartsWhenTheLogContainsAdditionalReadinessMessages()
    {
        // Given
        // The default wait strategy timeout is one hour, so bound the wait to keep a regression
        // from stalling the test run instead of failing it.
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));

        // When
        await _mongoDbContainer.StartAsync(cts.Token)
            .ConfigureAwait(true);

        // Then
        const string scriptContent = "rs.status().ok;";

        var execResult = await _mongoDbContainer.ExecScriptAsync(scriptContent, cts.Token)
            .ConfigureAwait(true);

        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }
}
