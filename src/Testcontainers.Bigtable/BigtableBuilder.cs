namespace Testcontainers.Bigtable;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class BigtableBuilder : ContainerBuilder<BigtableBuilder, BigtableContainer, BigtableConfiguration>
{
    public const string GoogleCloudCliImage = "gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators";

    public const ushort BigtablePort = 9000;

    /// <summary>
    /// Initializes a new instance of the <see cref="BigtableBuilder" /> class.
    /// </summary>
    public BigtableBuilder()
        : this(new BigtableConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigtableBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private BigtableBuilder(BigtableConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override BigtableConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override BigtableContainer Build()
    {
        Validate();
        return new BigtableContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override BigtableBuilder Init()
    {
        return base.Init()
            .WithImage(GoogleCloudCliImage)
            .WithPortBinding(BigtablePort, true)
            .WithEntrypoint("gcloud")
            .WithCommand("beta", "emulators", "bigtable", "start", "--host-port", "0.0.0.0:" + BigtablePort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*running.*$"));
    }

    /// <inheritdoc />
    protected override BigtableBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new BigtableConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override BigtableBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new BigtableConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override BigtableBuilder Merge(BigtableConfiguration oldValue, BigtableConfiguration newValue)
    {
        return new BigtableBuilder(new BigtableConfiguration(oldValue, newValue));
    }
}