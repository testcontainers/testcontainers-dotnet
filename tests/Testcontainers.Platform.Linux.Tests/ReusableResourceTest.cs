namespace Testcontainers.Tests;

public sealed class ReusableResourceTest : IAsyncLifetime, IDisposable
{
    private readonly DockerClient _dockerClient = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration(Guid.NewGuid()).CreateClient();

    private readonly FilterByProperty _filters = new FilterByProperty();

    private readonly IList<IAsyncDisposable> _disposables = new List<IAsyncDisposable>();

    private readonly string _labelKey = Guid.NewGuid().ToString("D");

    private readonly string _labelValue = Guid.NewGuid().ToString("D");

    public ReusableResourceTest()
    {
        _filters.Add("label", string.Join("=", _labelKey, _labelValue));
    }

    public async Task InitializeAsync()
    {
        for (var _ = 0; _ < 3; _++)
        {
            var container = new ContainerBuilder()
                .WithImage(CommonImages.Alpine)
                .WithEntrypoint(CommonCommands.SleepInfinity)
                .WithLabel(_labelKey, _labelValue)
                .WithReuse(true)
                .Build();

            var network = new NetworkBuilder()
                .WithName(_labelKey)
                .WithLabel(_labelKey, _labelValue)
                .WithReuse(true)
                .Build();

            var volume = new VolumeBuilder()
                .WithName(_labelKey)
                .WithLabel(_labelKey, _labelValue)
                .WithReuse(true)
                .Build();

            await Task.WhenAll(container.StartAsync(), network.CreateAsync(), volume.CreateAsync())
                .ConfigureAwait(false);

            _disposables.Add(container);
            _disposables.Add(network);
            _disposables.Add(volume);
        }
    }

    public Task DisposeAsync()
    {
        return Task.WhenAll(_disposables.Select(disposable => disposable.DisposeAsync().AsTask()));
    }

    public void Dispose()
    {
        _dockerClient.Dispose();
    }

    [Fact]
    public async Task ShouldReuseExistingResource()
    {
        var containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters { Filters = _filters })
            .ConfigureAwait(true);

        var networks = await _dockerClient.Networks.ListNetworksAsync(new NetworksListParameters { Filters = _filters })
            .ConfigureAwait(true);

        var response = await _dockerClient.Volumes.ListAsync(new VolumesListParameters { Filters = _filters })
            .ConfigureAwait(true);

        Assert.Single(containers);
        Assert.Single(networks);
        Assert.Single(response.Volumes);
    }

    public static class UnsupportedBuilderConfigurationTest
    {
        private const string EnabledCleanUpExceptionMessage = "Reuse cannot be used in conjunction with WithCleanUp(true). (Parameter 'Reuse')";

        private const string EnabledAutoRemoveExceptionMessage = "Reuse cannot be used in conjunction with WithAutoRemove(true). (Parameter 'Reuse')";

        public sealed class ContainerBuilderTest
        {
            [Fact]
            public void EnabledCleanUpThrowsException()
            {
                // Given
                var containerBuilder = new ContainerBuilder().WithReuse(true).WithCleanUp(true);

                // When
                var exception = Assert.Throws<ArgumentException>(() => containerBuilder.Build());

                // Then
                Assert.Equal(EnabledCleanUpExceptionMessage, exception.Message);
            }

            [Fact]
            public void EnabledAutoRemoveThrowsException()
            {
                // Given
                var containerBuilder = new ContainerBuilder().WithReuse(true).WithAutoRemove(true);

                // When
                var exception = Assert.Throws<ArgumentException>(() => containerBuilder.Build());

                // Then
                Assert.Equal(EnabledAutoRemoveExceptionMessage, exception.Message);
            }
        }

        public sealed class NetworkBuilderTest
        {
            [Fact]
            public void EnabledCleanUpThrowsException()
            {
                // Given
                var containerBuilder = new NetworkBuilder().WithReuse(true).WithCleanUp(true);

                // When
                var exception = Assert.Throws<ArgumentException>(() => containerBuilder.Build());

                // Then
                Assert.Equal(EnabledCleanUpExceptionMessage, exception.Message);
            }
        }

        public sealed class VolumeBuilderTest
        {
            [Fact]
            public void EnabledCleanUpThrowsException()
            {
                // Given
                var containerBuilder = new VolumeBuilder().WithReuse(true).WithCleanUp(true);

                // When
                var exception = Assert.Throws<ArgumentException>(() => containerBuilder.Build());

                // Then
                Assert.Equal(EnabledCleanUpExceptionMessage, exception.Message);
            }
        }
    }
}