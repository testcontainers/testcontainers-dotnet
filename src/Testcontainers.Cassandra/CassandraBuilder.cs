namespace Testcontainers.Cassandra;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class CassandraBuilder : ContainerBuilder<CassandraBuilder, CassandraContainer, CassandraConfiguration>
{
    public const string CassandraImage = "cassandra:5.0";

    public const ushort CqlPort = 9042;

    public const string DefaultDatacenterName = "dc1";

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraBuilder" /> class.
    /// </summary>
    public CassandraBuilder()
        : this(new CassandraConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private CassandraBuilder(CassandraConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override CassandraConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override CassandraContainer Build()
    {
        Validate();
        return new CassandraContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override CassandraBuilder Init()
    {
        return base.Init()
            .WithImage(CassandraImage)
            .WithPortBinding(CqlPort, true)
            .WithEnvironment("JVM_OPTS", "-Dcassandra.skip_wait_for_gossip_to_settle=0 -Dcassandra.initial_token=0")
            .WithEnvironment("HEAP_NEWSIZE", "128M")
            .WithEnvironment("MAX_HEAP_SIZE", "1024M")
            .WithEnvironment("CASSANDRA_SNITCH", "GossipingPropertyFileSnitch")
            .WithEnvironment("CASSANDRA_ENDPOINT_SNITCH", "GossipingPropertyFileSnitch")
            .WithEnvironment("CASSANDRA_DC", DefaultDatacenterName)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Startup complete"));
    }

    /// <inheritdoc />
    protected override CassandraBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CassandraConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CassandraBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CassandraConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CassandraBuilder Merge(CassandraConfiguration oldValue, CassandraConfiguration newValue)
    {
        return new CassandraBuilder(new CassandraConfiguration(oldValue, newValue));
    }
}