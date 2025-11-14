namespace Testcontainers.SpiceDB;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class SpiceDBBuilder : ContainerBuilder<SpiceDBBuilder, SpiceDBContainer, SpiceDBConfiguration>
{
    public const string SpiceDBImage = "authzed/spicedb:v1.45.1";

    public const ushort SpiceDBgRPCPort = 50051;

    public const ushort SpiceDBgHTTPPort = 8443;


    /// <summary>
    /// Initializes a new instance of the <see cref="SpiceDBBuilder" /> class.
    /// </summary>
    public SpiceDBBuilder()
        : this(new SpiceDBConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpiceDBBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private SpiceDBBuilder(SpiceDBConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override SpiceDBConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override SpiceDBContainer Build()
    {
        Validate();
        return new SpiceDBContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override SpiceDBBuilder Init()
    {
        return base.Init()
            .WithImage(SpiceDBImage)
            .WithPortBinding(SpiceDBgRPCPort, true)
            .WithPortBinding(SpiceDBgHTTPPort, true)
            .WithCommand("serve", $"--grpc-preshared-key={DockerResourceConfiguration.GrpcPresharedKey}", $"--datastore-engine={DockerResourceConfiguration.DatastoreEngine}", $"--log-level=info")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("grpc server started serving"));
    }

    /// <inheritdoc />
    protected override SpiceDBBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SpiceDBConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SpiceDBBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SpiceDBConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SpiceDBBuilder Merge(SpiceDBConfiguration oldValue, SpiceDBConfiguration newValue)
    {
        return new SpiceDBBuilder(new SpiceDBConfiguration(oldValue, newValue));
    }
}
