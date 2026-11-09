namespace Testcontainers.Tests;

public sealed class ComposableTest
{
    private static readonly ComposableEnumerable<string> AppendListFlag =
        new AppendEnumerable<string>(new[] { "-l" });

    private static readonly ComposableEnumerable<string> OverwriteWithSearchArg =
        new OverwriteEnumerable<string>(new[] { "pattern", "*.log" });

    [Fact]
    public void AppendsFlagAfterInitialArgument()
    {
        var command = new ComposableContainerBuilder()
            .WithCommand("foo")
            .WithCommand(AppendListFlag)
            .GetCommand();

        Assert.Equal(new[] { "foo", "-l" }, command);
    }

    [Fact]
    public void OverwritesArgumentCompletely()
    {
        var command = new ComposableContainerBuilder()
            .WithCommand("foo")
            .WithCommand(OverwriteWithSearchArg)
            .GetCommand();

        Assert.Equal(new[] { "pattern", "*.log" }, command);
    }

    [Fact]
    public void OverwritesThenAppendsFlag()
    {
        var command = new ComposableContainerBuilder()
            .WithCommand("foo")
            .WithCommand(OverwriteWithSearchArg)
            .WithCommand(AppendListFlag)
            .GetCommand();

        Assert.Equal(new[] { "pattern", "*.log", "-l" }, command);
    }

    [Fact]
    public void EnvironmentIsMergedByDefault()
    {
        var environments = new ComposableContainerBuilder()
            .WithEnvironment("A", "1")
            .WithEnvironment(new Dictionary<string, string> { { "A", "2" }, { "B", "3" } })
            .GetEnvironments();

        Assert.Equal(new Dictionary<string, string> { { "A", "2" }, { "B", "3" } }, environments);
    }

    [Fact]
    public void EnvironmentIsOverwritten()
    {
        var environments = new ComposableContainerBuilder()
            .WithEnvironment("A", "1")
            .WithEnvironment(new OverwriteDictionary<string, string>(new Dictionary<string, string> { { "B", "2" } }))
            .WithEnvironment("C", "3")
            .GetEnvironments();

        Assert.Equal(new Dictionary<string, string> { { "B", "2" }, { "C", "3" } }, environments);
    }

    [Fact]
    public void EnvironmentIsClearedByEmptyOverwrite()
    {
        var environments = new ComposableContainerBuilder()
            .WithEnvironment("A", "1")
            .WithEnvironment(new OverwriteDictionary<string, string>(new Dictionary<string, string>()))
            .GetEnvironments();

        Assert.Empty(environments);
    }

    [Fact]
    public void EnvironmentKeyIsRemoved()
    {
        var environments = new ComposableContainerBuilder()
            .WithEnvironment("A", "1")
            .WithEnvironment("ingest.geoip.downloader.enabled", "false")
            .WithEnvironment(new RemoveKey("ingest.geoip.downloader.enabled"))
            .GetEnvironments();

        Assert.Equal(new Dictionary<string, string> { { "A", "1" } }, environments);
    }

    private sealed class RemoveKey : ComposableDictionary<string, string>
    {
        private readonly string _key;

        public RemoveKey(string key) : base(null)
            => _key = key;

        public override ComposableDictionary<string, string> Compose(IReadOnlyDictionary<string, string> other)
            => new AppendDictionary<string, string>(other.Where(item => !_key.Equals(item.Key, StringComparison.Ordinal)).ToDictionary(item => item.Key, item => item.Value));
    }

    private sealed class ComposableContainerBuilder : ContainerBuilder<ComposableContainerBuilder, DockerContainer, ContainerConfiguration>
    {
        public ComposableContainerBuilder() : this(new ContainerConfiguration())
            => DockerResourceConfiguration = Init().DockerResourceConfiguration;

        private ComposableContainerBuilder(ContainerConfiguration configuration) : base(configuration)
            => DockerResourceConfiguration = configuration;

        protected override ContainerConfiguration DockerResourceConfiguration { get; }

        public IEnumerable<string> GetCommand()
            => DockerResourceConfiguration.Command;

        public IReadOnlyDictionary<string, string> GetEnvironments()
            => DockerResourceConfiguration.Environments;

        public override DockerContainer Build()
            => new(DockerResourceConfiguration);

        protected override ComposableContainerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
            => Merge(DockerResourceConfiguration, new ContainerConfiguration(resourceConfiguration));

        protected override ComposableContainerBuilder Clone(IContainerConfiguration resourceConfiguration)
            => Merge(DockerResourceConfiguration, new ContainerConfiguration(resourceConfiguration));

        protected override ComposableContainerBuilder Merge(ContainerConfiguration oldValue, ContainerConfiguration newValue)
            => new(new ContainerConfiguration(oldValue, newValue));
    }
}