namespace Testcontainers.DynamoDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class DynamoDbBuilder : ContainerBuilder<DynamoDbBuilder, DynamoDbContainer, DynamoDbConfiguration>
{
    [Obsolete("This image tag is not recommended: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string DynamoDbImage = "amazon/dynamodb-local:1.21.0";

    public const ushort DynamoDbPort = 8000;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public DynamoDbBuilder()
        : this(DynamoDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://hub.docker.com/r/amazon/dynamodb-local/tags">https://hub.docker.com/r/amazon/dynamodb-local/tags</see>.</param>
    public DynamoDbBuilder(string image)
        : this(new DynamoDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public DynamoDbBuilder(IImage image)
        : this(new DynamoDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private DynamoDbBuilder(DynamoDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override DynamoDbConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override DynamoDbContainer Build()
    {
        Validate();
        return new DynamoDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override DynamoDbBuilder Init()
    {
        return base.Init()
            .WithPortBinding(DynamoDbPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(DynamoDbPort).ForStatusCode(HttpStatusCode.BadRequest)));
    }

    /// <inheritdoc />
    protected override DynamoDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DynamoDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DynamoDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DynamoDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DynamoDbBuilder Merge(DynamoDbConfiguration oldValue, DynamoDbConfiguration newValue)
    {
        return new DynamoDbBuilder(new DynamoDbConfiguration(oldValue, newValue));
    }
}