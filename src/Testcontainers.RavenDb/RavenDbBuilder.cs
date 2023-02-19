namespace Testcontainers.RavenDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class RavenDbBuilder : ContainerBuilder<RavenDbBuilder, RavenDbContainer, RavenDbConfiguration>
{
    public const string RavenDbImage = "ravendb/ravendb:5.4-ubuntu-latest";

    public const ushort RavenDbPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbBuilder" /> class.
    /// </summary>
    public RavenDbBuilder()
        : this(new RavenDbConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private RavenDbBuilder(RavenDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override RavenDbConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override RavenDbContainer Build()
    {
        Validate();
        return new RavenDbContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override RavenDbBuilder Init()
    {
        return base.Init()
            .WithImage(RavenDbImage)
            .WithPortBinding(RavenDbPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()));
    }

    /// <inheritdoc />
    protected override RavenDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RavenDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RavenDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RavenDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RavenDbBuilder Merge(RavenDbConfiguration oldValue, RavenDbConfiguration newValue)
    {
        return new RavenDbBuilder(new RavenDbConfiguration(oldValue, newValue));
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var (stdout, _) = await container.GetLogsAsync(timestampsEnabled: false)
                .ConfigureAwait(false);

            return stdout.Contains("Server started");
        }
    }
}