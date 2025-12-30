namespace Testcontainers.Consul;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ConsulBuilder : ContainerBuilder<ConsulBuilder, ConsulContainer, ConsulConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string ConsulImage = "consul:1.15";

    public const int ConsulHttpPort = 8500;

    public const int ConsulGrpcPort = 8502;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsulBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public ConsulBuilder()
        : this(ConsulImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsulBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>consul:1.15</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/hashicorp/consul/tags" />.
    /// </remarks>
    public ConsulBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsulBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/hashicorp/consul/tags" />.
    /// </remarks>
    public ConsulBuilder(IImage image)
        : this(new ConsulConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsulBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ConsulBuilder(ConsulConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ConsulConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override ConsulContainer Build()
    {
        Validate();
        return new ConsulContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override ConsulBuilder Init()
    {
        return base.Init()
            .WithPortBinding(ConsulHttpPort, true)
            .WithPortBinding(ConsulGrpcPort, true)
            .WithCommand("agent", "-dev", "-client", "0.0.0.0")
            .WithCreateParameterModifier(cmd => cmd.HostConfig.CapAdd = new[] { "IPC_LOCK" })
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/v1/status/leader").ForPort(ConsulHttpPort)));
    }

    /// <inheritdoc />
    protected override ConsulBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ConsulConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ConsulBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ConsulConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ConsulBuilder Merge(ConsulConfiguration oldValue, ConsulConfiguration newValue)
    {
        return new ConsulBuilder(new ConsulConfiguration(oldValue, newValue));
    }
}