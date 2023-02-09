namespace Testcontainers.DynamoDB;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class DynamoDBBuilder : ContainerBuilder<DynamoDBBuilder, DynamoDBContainer, DynamoDBConfiguration>
{
    public const string DynamoDbImage = "amazon/dynamodb-local:1.21.0";

    public const ushort DynamoDbPort = 8000;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDBBuilder" /> class.
    /// </summary>
    public DynamoDBBuilder()
        : this(new DynamoDBConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDBBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private DynamoDBBuilder(DynamoDBConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override DynamoDBConfiguration DockerResourceConfiguration { get; }
    

    /// <inheritdoc />
    public override DynamoDBContainer Build()
    {
        Validate();
        return new DynamoDBContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override DynamoDBBuilder Init()
    {
        return base.Init()
            .WithImage(DynamoDbImage)
            .WithPortBinding(DynamoDbPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer());
    }

    /// <inheritdoc />
    protected override DynamoDBBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DynamoDBConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DynamoDBBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DynamoDBConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DynamoDBBuilder Merge(DynamoDBConfiguration oldValue, DynamoDBConfiguration newValue)
    {
        return new DynamoDBBuilder(new DynamoDBConfiguration(oldValue, newValue));
    }
}