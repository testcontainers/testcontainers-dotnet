namespace Testcontainers.DynamoDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class DynamoDbBuilder : ContainerBuilder<DynamoDbBuilder, DynamoDbContainer, DynamoDbConfiguration>
{
    public const string DynamoDbImage = "amazon/dynamodb-local:1.21.0";

    public const ushort DynamoDbPort = 8000;

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
            .WithImage(DynamoDbImage)
            .WithPortBinding(DynamoDbPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil(DynamoDbPort)));
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
    
    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        private readonly ushort _port;

        public WaitUntil(ushort port)
        {
            _port = port;
        }
        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var execResult = await container.ExecAsync(new[] { "curl", $"http://localhost:{_port}" })
                .ConfigureAwait(false);

            return 0L.Equals(execResult.ExitCode);
        }
    }
}