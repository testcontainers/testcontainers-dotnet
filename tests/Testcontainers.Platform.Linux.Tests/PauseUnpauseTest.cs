namespace Testcontainers.Tests;

public sealed class PauseUnpauseTest : IAsyncLifetime
{
    private readonly IContainer _container = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithCommand(CommonCommands.SleepInfinity)
        .Build();

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task PausesAndUnpausesContainerSuccessfully()
    {
        await _container.PauseAsync()
            .ConfigureAwait(true);
        Assert.Equal(TestcontainersStates.Paused, _container.State);

        await _container.UnpauseAsync()
            .ConfigureAwait(true);
        Assert.Equal(TestcontainersStates.Running, _container.State);
    }

    [Fact]
    public Task UnpausingRunningContainerThrowsDockerApiException()
    {
        return Assert.ThrowsAsync<DockerApiException>(() => _container.UnpauseAsync());
    }
}