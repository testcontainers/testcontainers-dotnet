namespace Testcontainers.Seq;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class SeqBuilder : ContainerBuilder<SeqBuilder, SeqContainer, SeqConfiguration>
{
    public const string SeqImage = "datalust/seq:2024.2.11456";

    public const ushort SeqApiPort = 80;
    public const ushort SeqIngestionPort = 5341;

    /// <inheritdoc />
    protected override SeqConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeqBuilder" /> class.
    /// </summary>
    public SeqBuilder()
        : this(new SeqConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeqBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private SeqBuilder(SeqConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    public override SeqContainer Build()
    {
        Validate();
        return new SeqContainer(DockerResourceConfiguration);
    }
    /// <inheritdoc />
    protected override SeqBuilder Init()
    {
        return base.Init()
           .WithImage(SeqImage)
           .WithPortBinding(SeqApiPort, true)
           .WithPortBinding(SeqIngestionPort, true)
           .WithEnvironment(new Dictionary<string, string>
           {
                {"ACCEPT_EULA", "Y" }
           })
           .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
               request.ForPath("/health").ForPort(SeqApiPort)));
    }

    /// <inheritdoc />
    protected override SeqBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SeqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SeqBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SeqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SeqBuilder Merge(SeqConfiguration oldValue, SeqConfiguration newValue)
    {
        return new SeqBuilder(new SeqConfiguration(oldValue, newValue));
    }
}