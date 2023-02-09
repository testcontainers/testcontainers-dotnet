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
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil(DynamoDbPort)));
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
    
    /// <inheritdoc cref="IWaitUntil" />
    /// <remarks>
    /// Uses the sqlcmd utility scripting variables to detect readiness of the MsSql container:
    /// https://learn.microsoft.com/en-us/sql/tools/sqlcmd/sqlcmd-utility?view=sql-server-linux-ver15#sqlcmd-scripting-variables.
    /// </remarks>
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
