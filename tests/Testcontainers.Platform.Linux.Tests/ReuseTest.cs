using DotNet.Testcontainers.Networks;
using DotNet.Testcontainers.Volumes;

namespace Testcontainers.Tests;

public sealed class ReuseTest : IAsyncLifetime
{
    private const string LabelKey = "org.testcontainers.reuse-test";
    private const string LabelValue = "true";
    private const int ContainersToCreate = 3;

    private readonly List<IContainer> _containers = new();
    private readonly List<IVolume> _volumes = new();
    private readonly List<INetwork> _networks = new();

    private static IVolume BuildVolume()
    {
        return new VolumeBuilder()
            .WithName("reuse-test-volume")
            .WithLabel(LabelKey, LabelValue)
            .WithReuse(true)
            .Build();
    }

    private static INetwork BuildNetwork()
    {
        return new NetworkBuilder()
            .WithName("reuse-test-network")
            .WithDriver(NetworkDriver.Host)
            .WithLabel(LabelKey, LabelValue)
            .WithReuse(true)
            .Build();
    }

    private static IContainer BuildContainer(IVolume volume, INetwork network)
    {
        return new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithLabel(LabelKey, LabelValue)
            .WithAutoRemove(false)
            .WithCleanUp(false)
            .WithEntrypoint("sleep")
            .WithCommand("infinity")
            .WithNetwork(network)
            .WithVolumeMount(volume, "/test", AccessMode.ReadWrite)
            .WithReuse(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        for (var i = 0; i < ContainersToCreate; i++)
        {
            var network = BuildNetwork();
            await network.CreateAsync();
            _networks.Add(network);

            var volume = BuildVolume();
            await volume.CreateAsync();
            _volumes.Add(volume);

            var container = BuildContainer(volume, network);
            await container.StartAsync();
            _containers.Add(container);
        }
    }

    public async Task DisposeAsync()
    {
        using var clientConfiguration = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration(ResourceReaper.DefaultSessionId);
        using var client = clientConfiguration.CreateClient();

        var distinctContainers = _containers.DistinctBy(container => container.Id).ToList();
        await Task.WhenAll(distinctContainers.Select(container => container.DisposeAsync().AsTask()));
        await Task.WhenAll(distinctContainers.Select(container => client.Containers.RemoveContainerAsync(container.Id, new())));

        var distinctVolumes = _volumes.DistinctBy(volume => volume.Name).ToList();
        await Task.WhenAll(distinctVolumes.Select(volume => volume.DisposeAsync().AsTask()));

        var distinctNetworks = _networks.DistinctBy(network => network.Name).ToList();
        await Task.WhenAll(distinctNetworks.Select(network => network.DisposeAsync().AsTask()));
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
        var networksListParameters = new NetworksListParameters { Filters = filters };
        var volumeListParameters = new VolumesListParameters { Filters = filters };

        // When
        var containers = await client.Containers.ListContainersAsync(containersListParameters);
        var networks = await client.Networks.ListNetworksAsync(networksListParameters);
        var volumes = await client.Volumes.ListAsync(volumeListParameters);

        // Then
        Assert.Single(containers);
        Assert.Single(networks);
        Assert.Single(volumes.Volumes);
    }
}