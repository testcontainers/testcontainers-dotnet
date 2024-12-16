namespace Testcontainers.Tests
{
    public abstract class PauseUnpauseTest
    {
        [Fact]
        public async Task PauseContainer()
        {
            await using var container = new ContainerBuilder()
                .WithImage(CommonImages.Alpine)
                .WithEntrypoint(CommonCommands.SleepInfinity)
                .Build();

            await container.StartAsync()
                .ConfigureAwait(true);

            await container.PauseAsync().ConfigureAwait(true);

            Assert.Equal(TestcontainersStates.Paused, container.State);

            await container.UnpauseAsync().ConfigureAwait(true);

            Assert.Equal(TestcontainersStates.Running, container.State);
        }
    }
}