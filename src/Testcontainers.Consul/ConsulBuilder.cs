using DotNet.Testcontainers.Images;

namespace Testcontainers.Consul;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ConsulBuilder : ContainerBuilder<ConsulBuilder, ConsulContainer, ConsulConfiguration>
{
    public const string ConsulImage = "consul:1.15";

    public const int ConsulHttpPort = 8500;

    public const int ConsulGrpcPort = 8502;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsulBuilder" /> class.
    /// </summary>
    [Obsolete("Use constructor with image as a parameter instead.")]
    public ConsulBuilder()
        : this(new ConsulConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(ConsulImage).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsulBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://hub.docker.com/_/consul/tags">https://hub.docker.com/_/consul/tags</see>.</param>
    public ConsulBuilder(string image)
        : this(new ConsulConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsulBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
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
            .WithImage(ConsulImage)
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