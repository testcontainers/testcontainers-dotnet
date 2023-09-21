namespace Testcontainers.PubSub;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PubSubBuilder : ContainerBuilder<PubSubBuilder, PubSubContainer, PubSubConfiguration>
{
    public const string PubSubPort = "8085";
    const string Image = "gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators";
    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubBuilder" /> class.
    /// </summary>
    public PubSubBuilder()
        : this(new PubSubConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private PubSubBuilder(PubSubConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override PubSubConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the PubSub config.
    // /// </summary>
    // /// <param name="config">The PubSub config.</param>
    // /// <returns>A configured instance of <see cref="PubSubBuilder" />.</returns>
    // public PubSubBuilder WithPubSubConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in PubSubConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new PubSubConfiguration(config: config));
    // }

    protected override PubSubConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override PubSubContainer Build()
    {
        Validate();
        return new PubSubContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override PubSubBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override PubSubBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PubSubConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PubSubBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PubSubConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PubSubBuilder Merge(PubSubConfiguration oldValue, PubSubConfiguration newValue)
    {
        return new PubSubBuilder(new PubSubConfiguration(oldValue, newValue));
    }

    protected override PubSubBuilder Init()
    {
        return base.Init()
                .WithImage(Image)
                .WithPortBinding(PubSubPort,true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("started."))
                .WithEntrypoint("gcloud")
                .WithCommand("beta", "emulators", "pubsub", "start", "--host-port", "0.0.0.0:" + PubSubPort) 
            ;
    }
}