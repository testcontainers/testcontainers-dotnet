namespace Testcontainers.Tests;

public sealed class DependsOnTests : IAsyncLifetime
{
    private const string DependsOnKey = "org.testcontainers.depends-on";

    private const string DependsOnValue = "true";

    private static readonly IDictionary<string, bool> LabelFilter = new Dictionary<string, bool> { { string.Join("=", DependsOnKey, DependsOnValue), true } };

    private readonly IContainer _container = new ContainerBuilder()
        .DependsOn(new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithLabel(DependsOnKey, DependsOnValue)
            .Build())
        .DependsOn(new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithLabel(DependsOnKey, DependsOnValue)
            .Build())
        .WithNetwork(new NetworkBuilder()
            .WithLabel(DependsOnKey, DependsOnValue)
            .Build())
        .WithVolumeMount(new VolumeBuilder()
            .WithLabel(DependsOnKey, DependsOnValue)
            .Build(), "/workdir")
        .WithImage(CommonImages.Alpine)
        .WithLabel(DependsOnKey, DependsOnValue)
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
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task DependsOnCreatesDependentResources()
    {
        // Given
        using var clientConfiguration = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration(ResourceReaper.DefaultSessionId);

        using var client = clientConfiguration.CreateClient();

        var containersListParameters = new ContainersListParameters { All = true, Filters = new Dictionary<string, IDictionary<string, bool>> { { "label", LabelFilter } } };

        var networksListParameters = new NetworksListParameters { Filters = new Dictionary<string, IDictionary<string, bool>> { { "label", LabelFilter } } };

        var volumesListParameters = new VolumesListParameters { Filters = new Dictionary<string, IDictionary<string, bool>> { { "label", LabelFilter } } };

        // When
        var containers = await client.Containers.ListContainersAsync(containersListParameters)
            .ConfigureAwait(false);

        var networks = await client.Networks.ListNetworksAsync(networksListParameters)
            .ConfigureAwait(false);

        var volumesListResponse = await client.Volumes.ListAsync(volumesListParameters)
            .ConfigureAwait(false);

        // Then
        Assert.Equal(3, containers.Count);
        Assert.Equal(1, networks.Count);
        Assert.Equal(1, volumesListResponse.Volumes.Count);
    }
}