namespace Testcontainers.Tests;

public sealed class ReuseTest : IAsyncLifetime
{
    private const string LabelKey = "org.testcontainers.reuse-test";
    private const string LabelValue = "true";
    private const int ContainersToCreate = 3;

    private readonly List<IContainer> _containers = new();

    private static IContainer CreateContainer()
    {
        return new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithLabel(LabelKey, LabelValue)
            .WithAutoRemove(false)
            .WithCleanUp(false)
            .WithEntrypoint("sleep")
            .WithCommand("infinity")
            .WithReuse(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        for (var i = 0; i < ContainersToCreate; i++)
        {
            var container = CreateContainer();
            await container.StartAsync();
            _containers.Add(container);
        }
    }

    public async Task DisposeAsync()
    {
        var distinctContainers = _containers.DistinctBy(container => container.Id).ToList();
        await Task.WhenAll(distinctContainers.Select(container => container.DisposeAsync().AsTask()));

        using var clientConfiguration = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration(ResourceReaper.DefaultSessionId);
        using var client = clientConfiguration.CreateClient();
        await Task.WhenAll(distinctContainers.Select(container => client.Containers.RemoveContainerAsync(container.Id, new())));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReusesContainer()
    {
        // Given
        using var clientConfiguration = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration(ResourceReaper.DefaultSessionId);
        using var client = clientConfiguration.CreateClient();

        var labelFilter = new Dictionary<string, bool> { { $"{LabelKey}={LabelValue}", true } };
        var filters = new Dictionary<string, IDictionary<string, bool>> { { "label", labelFilter } };
        var containersListParameters = new ContainersListParameters { All = true, Filters = filters };

        // When
        var containers = await client.Containers.ListContainersAsync(containersListParameters)
            .ConfigureAwait(false);

        // Then
        Assert.Single(containers);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ThrowsIfWithReuseIsNotLastCalledMethod()
    {
        Assert.Throws<ArgumentException>(() => new ContainerBuilder()
            .WithReuse(true)
            .WithImage(CommonImages.Alpine)
            .Build());
    }
}