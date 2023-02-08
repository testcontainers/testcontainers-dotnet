namespace Testcontainers.DynamoDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class DynamoDbBuilder : ContainerBuilder<DynamoDbBuilder, DynamoDbContainer, DynamoDbConfiguration>
{
    public const string DynaliteImage = "quay.io/testcontainers/dynalite:v1.2.1-1";

    public const ushort DynalitePort = 4567;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbBuilder" /> class.
    /// </summary>
    public DynamoDbBuilder()
        : this(new DynamoDbConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private DynamoDbBuilder(DynamoDbConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override DynamoDbConfiguration DockerResourceConfiguration { get; }
    

    /// <inheritdoc />
    public override DynamoDbContainer Build()
    {
        Validate();
        return new DynamoDbContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override DynamoDbBuilder Init()
    {
        return base.Init()
            .WithImage(DynaliteImage)
            .WithPortBinding(DynalitePort, true)
            .WithWaitStrategy(Wait.ForUnixContainer());
    }

    /// <inheritdoc />
    protected override DynamoDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DynamoDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DynamoDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DynamoDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DynamoDbBuilder Merge(DynamoDbConfiguration oldValue, DynamoDbConfiguration newValue)
    {
        return new DynamoDbBuilder(new DynamoDbConfiguration(oldValue, newValue));
    }
}