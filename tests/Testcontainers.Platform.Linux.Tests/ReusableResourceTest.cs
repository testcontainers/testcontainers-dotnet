namespace Testcontainers.Tests;

// We cannot run these tests in parallel because they interfere with the port
// forwarding tests. When the port forwarding container is running, Testcontainers
// automatically inject the necessary extra hosts into the builder configuration
// using `WithPortForwarding()` internally. Depending on when the test framework
// starts the port forwarding container, these extra hosts can lead to flakiness.
// This happens because the reuse hash changes, resulting in two containers with
// the same labels running instead of one.
[CollectionDefinition(nameof(ReusableResourceTest), DisableParallelization = true)]
[Collection(nameof(ReusableResourceTest))]
public sealed class ReusableResourceTest : IAsyncLifetime
{
    private readonly FilterByProperty _filters = new FilterByProperty();

    private readonly IList<IAsyncDisposable> _disposables = new List<IAsyncDisposable>();

    private readonly string _labelKey = Guid.NewGuid().ToString("D");

    private readonly string _labelValue = Guid.NewGuid().ToString("D");

    public ReusableResourceTest()
    {
        _filters.Add("label", string.Join("=", _labelKey, _labelValue));
    }

    public async ValueTask InitializeAsync()
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

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(_disposables
            .Take(3)
            .Select(disposable =>
            {
                // We do not want to leak resources, but `WithCleanUp(true)` cannot be used
                // alongside `WithReuse(true)`. As a workaround, we set the `SessionId` using
                // reflection afterward to delete the container, network, and volume.
                disposable.AsDynamic()._configuration.SessionId = ResourceReaper.DefaultSessionId;
                return disposable.DisposeAsync().AsTask();
            }));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShouldReuseExistingResource()
    {
        using var clientConfiguration = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration(Guid.NewGuid());

        using var client = clientConfiguration.CreateClient();

        var containersListParameters = new ContainersListParameters { All = true, Filters = _filters };

        var networksListParameters = new NetworksListParameters { Filters = _filters };

        var volumesListParameters = new VolumesListParameters { Filters = _filters };

        var containers = await client.Containers.ListContainersAsync(containersListParameters, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var networks = await client.Networks.ListNetworksAsync(networksListParameters, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var response = await client.Volumes.ListAsync(volumesListParameters, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        Assert.Single(containers);
        Assert.Single(networks);
        Assert.Single(response.Volumes);
    }

    public static class ReuseHashTest
    {
        public sealed class EqualTest
        {
            [Fact]
            [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
            public void ForSameConfigurationCreatedInDifferentOrder()
            {
                var env1 = new Dictionary<string, string>
                {
                    ["keyA"] = "valueA",
                    ["keyB"] = "valueB",
                };
                var env2 = new Dictionary<string, string>
                {
                    ["keyB"] = "valueB",
                    ["keyA"] = "valueA",
                };
                var hash1 = new ReuseHashContainerBuilder().WithEnvironment(env1).WithLabel("labelA", "A").WithLabel("labelB", "B").GetReuseHash();
                var hash2 = new ReuseHashContainerBuilder().WithEnvironment(env2).WithLabel("labelB", "B").WithLabel("labelA", "A").GetReuseHash();
                Assert.Equal(hash1, hash2);
            }

            [Fact]
            [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
            public void ForGivenConfiguration()
            {
                var env = new Dictionary<string, string>
                {
                    ["keyB"] = "valueB",
                    ["keyA"] = "valueA",
                };
                var hash = new ReuseHashContainerBuilder().WithEnvironment(env).WithLabel("labelB", "B").WithLabel("labelA", "A").GetReuseHash();

                // 50MEP+vnxEkQFo5PrndJ7oKOfh8= is the base64 encoded SHA1 of this JSON:
                // {"Image":null,"Name":null,"Entrypoint":null,"Command":[],"Environments":{"keyA":"valueA","keyB":"valueB"},"ExposedPorts":{},"PortBindings":{},"NetworkAliases":[],"ExtraHosts":[],"Labels":{"labelA":"A","labelB":"B","org.testcontainers":"true","org.testcontainers.lang":"dotnet"}}
                Assert.Equal("50MEP+vnxEkQFo5PrndJ7oKOfh8=", hash);
            }
        }

        public sealed class NotEqualTest
        {
            [Fact]
            [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
            public void ForDifferentNames()
            {
                var hash1 = new ReuseHashContainerBuilder().WithName("Name1").GetReuseHash();
                var hash2 = new ReuseHashContainerBuilder().WithName("Name2").GetReuseHash();
                Assert.NotEqual(hash1, hash2);
            }
        }
    }

    public static class UnsupportedBuilderConfigurationTest
    {
        private const string EnabledCleanUpExceptionMessage = "Reuse cannot be used in conjunction with WithCleanUp(true). (Parameter 'Reuse')";

        private const string EnabledAutoRemoveExceptionMessage = "Reuse cannot be used in conjunction with WithAutoRemove(true). (Parameter 'Reuse')";

        public sealed class ContainerBuilderTest
        {
            [Fact]
            [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
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
            [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
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
            [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
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
            [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
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

    private sealed class ReuseHashContainerBuilder : ContainerBuilder<ReuseHashContainerBuilder, DockerContainer, ContainerConfiguration>
    {
        public ReuseHashContainerBuilder() : this(new ContainerConfiguration())
            => DockerResourceConfiguration = Init().DockerResourceConfiguration;

        private ReuseHashContainerBuilder(ContainerConfiguration configuration) : base(configuration)
            => DockerResourceConfiguration = configuration;

        protected override ContainerConfiguration DockerResourceConfiguration { get; }

        public string GetReuseHash()
            => DockerResourceConfiguration.GetReuseHash();

        public override DockerContainer Build()
            => new(DockerResourceConfiguration);

        protected override ReuseHashContainerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
            => Merge(DockerResourceConfiguration, new ContainerConfiguration(resourceConfiguration));

        protected override ReuseHashContainerBuilder Clone(IContainerConfiguration resourceConfiguration)
            => Merge(DockerResourceConfiguration, new ContainerConfiguration(resourceConfiguration));

        protected override ReuseHashContainerBuilder Merge(ContainerConfiguration oldValue, ContainerConfiguration newValue)
            => new(new ContainerConfiguration(oldValue, newValue));
    }
}