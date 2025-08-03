namespace Testcontainers.Tests;

public sealed class ComposableTest
{
    private const string InitCommand = "foo";

    private static readonly ComposableEnumerable<string> AppendOnly =
        new AppendOnlyEnumerable<string>(new[] { "bar" });

    private static readonly ComposableEnumerable<string> Overwrite =
        new OverwriteEnumerable<string>(new[] { "baz" });

    [Fact]
    public void AppendsBarAfterFoo_WhenAppendOnlyAfterInitAndFoo()
    {
        var command = new ComposableContainerBuilder()
            .WithCommand(InitCommand)
            .WithCommand(AppendOnly)
            .GetCommand();

        Assert.Equal(new[] { "foo", "bar" }, command);
    }

    [Fact]
    public void OverwritesWithBar_WhenOverwriteIsLastAfterInitAndFoo()
    {
        var command = new ComposableContainerBuilder()
            .WithCommand(InitCommand)
            .WithCommand(Overwrite)
            .GetCommand();

        Assert.Equal(new[] { "baz" }, command);
    }

    [Fact]
    public void AppendsBazAfterBar_WhenAppendOnlyIsLastAfterOverwrite()
    {
        var command = new ComposableContainerBuilder()
            .WithCommand(InitCommand)
            .WithCommand(Overwrite)
            .WithCommand(AppendOnly)
            .GetCommand();

        Assert.Equal(new[] { "baz", "bar" }, command);
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