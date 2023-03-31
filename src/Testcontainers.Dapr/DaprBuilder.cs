namespace Testcontainers.Dapr;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class DaprBuilder : ContainerBuilder<DaprBuilder, DaprContainer, DaprConfiguration>
{
    public const string DaprImage = "daprio/daprd:1.10.4";

    public const int AppPort = 80;

    public const int DaprHttpPort = 3500;

    public const int DaprGrpcPort = 50001;

    public const string LogLevel = "info";


    /// <summary>
    /// Initializes a new instance of the <see cref="DaprBuilder" /> class.
    /// </summary>
    public DaprBuilder()
        : this(new DaprConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DaprBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private DaprBuilder(DaprConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override DaprConfiguration DockerResourceConfiguration { get; }


    public DaprBuilder WithAppId(string appId)
    {
        return Merge(DockerResourceConfiguration, new DaprConfiguration(appId: appId))
            .WithCommand("--app-id", appId);
    }

    // public DaprBuilder WithAppPort(int appPort)
    // {
    //     return Merge(DockerResourceConfiguration, new DaprConfiguration(appPort: appPort))
    //         .WithCommand("--app-port", appPort.ToString())
    //         .WithPortBinding(appPort, true);
    // }

    public DaprBuilder WithDaprHttpPort(int daprHttpPort)
    {

        return Merge(DockerResourceConfiguration, new DaprConfiguration())
            .WithCommand("--dapr-http-port", daprHttpPort.ToString())
            .WithPortBinding(daprHttpPort, true);
            //.WithExposedPort(daprHttpPort);
    }

    public DaprBuilder WithDaprGrpcPort(int daprGrpcPort)
    {
        return Merge(DockerResourceConfiguration, new DaprConfiguration())
            .WithCommand("--dapr-grpc-port", daprGrpcPort.ToString())
            .WithPortBinding(daprGrpcPort, true);
            //.WithExposedPort(daprGrpcPort);
    }

    // public DaprBuilder WithLogLevel(string logLevel)
    // {
    //     return Merge(DockerResourceConfiguration, new DaprConfiguration(logLevel: logLevel))
    //         .WithCommand("--log-level", logLevel);
    // }

    /// <inheritdoc />
    public override DaprContainer Build()
    {
        Validate();

        // var daprBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 
        // ? this 
        // : WithWaitStrategy(
        //         Wait.ForUnixContainer()
        //         .UntilHttpRequestIsSucceeded(request =>  
        //         request.ForPort(DaprHttpPort)
        //             .ForPath("/v1.0/healthz")
        //             .ForStatusCode(System.Net.HttpStatusCode.NoContent)));

        return new DaprContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override DaprBuilder Init()
    {       
        return base.Init()
            .WithImage(DaprImage)
            .WithCommand("./daprd")
            .WithPortBinding(DaprHttpPort, true)
            .WithPortBinding(DaprGrpcPort, true);
            //.WithLogLevel(LogLevel);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
         base.Validate();

        // _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
        //     .NotNull()
        //     .NotEmpty();
    }

    /// <inheritdoc />
    protected override DaprBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DaprConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DaprBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DaprConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DaprBuilder Merge(DaprConfiguration oldValue, DaprConfiguration newValue)
    {
        return new DaprBuilder(new DaprConfiguration(oldValue, newValue));
    }
}