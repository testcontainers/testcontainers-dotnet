using Docker.DotNet;
using Xunit.Sdk;

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
    public async Task Unpauses_NotPaused_ContainerSuccessfully()
    {
        var thrown = await Assert.ThrowsAsync<DockerApiException>(async () => await _container.UnpauseAsync().ConfigureAwait(true));
        
        Assert.Equal(HttpStatusCode.InternalServerError, thrown.StatusCode);
        Assert.Equal($"Docker API responded with status code=InternalServerError, response={{\"message\":\"Container {_container.Id} is not paused\"}}\n", thrown.Message);
    }
}