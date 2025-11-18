namespace Testcontainers.Tests;

public sealed class PauseUnpauseTest : IAsyncLifetime
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
    public async Task PausesAndUnpausesContainerSuccessfully()
    {
        await _container.PauseAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        Assert.Equal(TestcontainersStates.Paused, _container.State);

        await _container.UnpauseAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        Assert.Equal(TestcontainersStates.Running, _container.State);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public Task UnpausingRunningContainerThrowsDockerApiException()
    {
        return Assert.ThrowsAsync<DockerApiException>(() => _container.UnpauseAsync(TestContext.Current.CancellationToken));
    }
}