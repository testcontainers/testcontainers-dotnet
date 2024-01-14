namespace Testcontainers.Tests;

public sealed class DependsOnTest : IAsyncLifetime
{
    private const string DependsOnKey = "org.testcontainers.depends-on";

    private const string DependsOnValue = "true";

    private readonly IContainer _container = new ContainerBuilder()
        .DependsOn(new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithLabel(DependsOnKey, DependsOnValue)
            .Build())
        .DependsOn(new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithLabel(DependsOnKey, DependsOnValue)
            .Build())
        .DependsOn(new NetworkBuilder()
            .WithLabel(DependsOnKey, DependsOnValue)
            .Build())
        .DependsOn(new VolumeBuilder()
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

        var labelFilter = new Dictionary<string, bool> { { string.Join("=", DependsOnKey, DependsOnValue), true } };

        var filters = new Dictionary<string, IDictionary<string, bool>> { { "label", labelFilter } };

        var containersListParameters = new ContainersListParameters { All = true, Filters = filters };

        var networksListParameters = new NetworksListParameters { Filters = filters };

        var volumesListParameters = new VolumesListParameters { Filters = filters };

        // When
        var containers = await client.Containers.ListContainersAsync(containersListParameters)
            .ConfigureAwait(true);

        var networks = await client.Networks.ListNetworksAsync(networksListParameters)
            .ConfigureAwait(true);

        var volumesListResponse = await client.Volumes.ListAsync(volumesListParameters)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(3, containers.Count);
        Assert.Single(networks);
        Assert.Single(volumesListResponse.Volumes);
    }
}