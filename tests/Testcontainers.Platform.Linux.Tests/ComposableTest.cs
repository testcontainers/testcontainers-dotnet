namespace Testcontainers.Tests;

public sealed class ComposableTest
{
    private static readonly ComposableEnumerable<string> AppendListFlag =
        new AppendEnumerable<string>(new[] { "-l" });

    private static readonly ComposableEnumerable<string> OverwriteWithSearchArg =
        new OverwriteEnumerable<string>(new[] { "pattern", "*.log" });

    private static readonly ComposableDictionary<string, string> AppendBarVariable =
        new AppendDictionary<string, string>(new Dictionary<string, string> { { "BAR", "bar" } });

    private static readonly ComposableDictionary<string, string> OverwriteWithBazVariable =
        new OverwriteDictionary<string, string>(new Dictionary<string, string> { { "BAZ", "baz" } });

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
    public void AppendsVariableAfterInitialVariable()
    {
        var environments = new ComposableContainerBuilder()
            .WithEnvironment("FOO", "foo")
            .WithEnvironment(AppendBarVariable)
            .GetEnvironments();

        Assert.Equal(new Dictionary<string, string> { { "FOO", "foo" }, { "BAR", "bar" } }, environments);
    }

    [Fact]
    public void OverwritesVariablesCompletely()
    {
        var environments = new ComposableContainerBuilder()
            .WithEnvironment("FOO", "foo")
            .WithEnvironment(OverwriteWithBazVariable)
            .GetEnvironments();

        Assert.Equal(new Dictionary<string, string> { { "BAZ", "baz" } }, environments);
    }

    [Fact]
    public void OverwritesThenAppendsVariable()
    {
        var environments = new ComposableContainerBuilder()
            .WithEnvironment("FOO", "foo")
            .WithEnvironment(OverwriteWithBazVariable)
            .WithEnvironment(AppendBarVariable)
            .GetEnvironments();

        Assert.Equal(new Dictionary<string, string> { { "BAZ", "baz" }, { "BAR", "bar" } }, environments);
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