namespace Testcontainers.Tests;

public sealed class DependsOnTest : IAsyncLifetime
{
    private readonly FilterByProperty _filters = new FilterByProperty();

    private readonly IList<IAsyncDisposable> _disposables = new List<IAsyncDisposable>();

    private readonly string _labelKey = Guid.NewGuid().ToString("D");

    private readonly string _labelValue = Guid.NewGuid().ToString("D");

    public DependsOnTest()
    {
        _filters.Add("label", string.Join("=", _labelKey, _labelValue));
    }

    public async ValueTask InitializeAsync()
    {
        var childContainer1 = new ContainerBuilder(CommonImages.Alpine)
            .WithLabel(_labelKey, _labelValue)
            .Build();

        var childContainer2 = new ContainerBuilder(CommonImages.Alpine)
            .WithLabel(_labelKey, _labelValue)
            .Build();

        var network = new NetworkBuilder()
            .WithLabel(_labelKey, _labelValue)
            .Build();

        var volume = new VolumeBuilder()
            .WithLabel(_labelKey, _labelValue)
            .Build();

        var parentContainer = new ContainerBuilder(CommonImages.Alpine)
            .DependsOn(childContainer1)
            .DependsOn(childContainer2)
            .DependsOn(network)
            .DependsOn(volume, "/workdir")
            .WithLabel(_labelKey, _labelValue)
            .Build();

        await parentContainer.StartAsync()
            .ConfigureAwait(false);

        _disposables.Add(parentContainer);
        _disposables.Add(childContainer1);
        _disposables.Add(childContainer2);
        _disposables.Add(network);
        _disposables.Add(volume);
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(_disposables.Select(disposable => disposable.DisposeAsync().AsTask()))
            .ConfigureAwait(false);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task DependsOnCreatesDependentResources()
    {
        // Given
        using var clientConfiguration = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration(Guid.NewGuid());

        using var client = clientConfiguration.CreateClient();

        var containersListParameters = new ContainersListParameters { All = true, Filters = _filters };

        var networksListParameters = new NetworksListParameters { Filters = _filters };

        var volumesListParameters = new VolumesListParameters { Filters = _filters };

        // When
        var containers = await client.Containers.ListContainersAsync(containersListParameters, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var networks = await client.Networks.ListNetworksAsync(networksListParameters, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var response = await client.Volumes.ListAsync(volumesListParameters, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(3, containers.Count);
        Assert.Single(networks);
        Assert.Single(response.Volumes);
    }
}