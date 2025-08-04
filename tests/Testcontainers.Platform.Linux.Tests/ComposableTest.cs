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

    private sealed class ComposableContainerBuilder : ContainerBuilder<ComposableContainerBuilder, DockerContainer, ContainerConfiguration>
    {
        public ComposableContainerBuilder() : this(new ContainerConfiguration())
            => DockerResourceConfiguration = Init().DockerResourceConfiguration;

        private ComposableContainerBuilder(ContainerConfiguration configuration) : base(configuration)
            => DockerResourceConfiguration = configuration;

        protected override ContainerConfiguration DockerResourceConfiguration { get; }

        public IEnumerable<string> GetCommand()
            => DockerResourceConfiguration.Command;

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