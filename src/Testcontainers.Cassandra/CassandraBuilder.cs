namespace Testcontainers.Cassandra;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class CassandraBuilder : ContainerBuilder<CassandraBuilder, CassandraContainer, CassandraConfiguration>
{
    [Obsolete("This image tag is not recommended: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string CassandraImage = "cassandra:5.0";

    public const ushort CqlPort = 9042;

    public const string DefaultDatacenterName = "dc1";

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public CassandraBuilder()
        : this(CassandraImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>cassandra:5.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/cassandra/tags" />.
    /// </remarks>
    public CassandraBuilder(string image)
        : this(new CassandraConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CassandraBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/cassandra/tags" />.
    /// </remarks>
    public CassandraBuilder(IImage image)
        : this(new CassandraConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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