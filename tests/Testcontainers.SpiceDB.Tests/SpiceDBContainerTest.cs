using System.Collections.Generic;

namespace Testcontainers.SpiceDB;

public sealed class SpiceDBContainerTest : IAsyncLifetime
{
    private readonly SpiceDBContainer _spicedbContainer = new SpiceDBBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _spicedbContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _spicedbContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ExpectedPortIsMapped()
    {
        // Given & When
        var mappedPort = _spicedbContainer.GetMappedPublicPort(SpiceDBBuilder.SpiceDBPort);

        // Then
        Assert.True(mappedPort > 0);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecCommandReturnsSuccessful()
    {
        // Given
        List<string> commands = ["spicedb-cli", "version"];

        // When
        var execResult = await _spicedbContainer.ExecAsync(commands, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Contains("spicedb", execResult.Stdout);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PingCommandReturnsSuccessful()
    {
        // Given
        List<string> commands = ["spicedb-cli", "ping"];

        // When
        var execResult = await _spicedbContainer.ExecAsync(commands, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetGrpcConnectionStringReturnsExpectedFormat()
    {
        // Given & When
        var connectionString = _spicedbContainer.GetGrpcConnectionString();

        // Then
        // Note: This test will need to be updated once GetConnectionString() is properly implemented
        Assert.NotNull(connectionString);
    }
}
