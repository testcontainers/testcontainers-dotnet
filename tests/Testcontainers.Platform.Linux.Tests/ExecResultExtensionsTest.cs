namespace Testcontainers.Tests;

public sealed class ExecResultExtensionsTest : IAsyncLifetime
{
    private readonly IContainer _container = new ContainerBuilder(CommonImages.Alpine)
        .WithCommand(CommonCommands.SleepInfinity)
        .Build();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _container.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecAsyncShouldSucceedWhenCommandReturnsZeroExitCode()
    {
        // Given
        var command = new[] { "true" };

        // When
        var exception = await Record.ExceptionAsync(() => _container.ExecAsync(command, TestContext.Current.CancellationToken).ThrowOnFailure())
            .ConfigureAwait(true);

        // Then
        Assert.Null(exception);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecAsyncShouldThrowExecFailedExceptionWhenCommandFails()
    {
        // Given
        var command = new[] { "/bin/sh", "-c", "echo out; echo err >&2; exit 1" };

        // When
        var exception = await Assert.ThrowsAsync<ExecFailedException>(() => _container.ExecAsync(command, TestContext.Current.CancellationToken).ThrowOnFailure())
            .ConfigureAwait(true);

        // Then
        Assert.Equal(1, exception.ExecResult.ExitCode);
        Assert.Equal("out", exception.ExecResult.Stdout.Trim());
        Assert.Equal("err", exception.ExecResult.Stderr.Trim());
    }
}