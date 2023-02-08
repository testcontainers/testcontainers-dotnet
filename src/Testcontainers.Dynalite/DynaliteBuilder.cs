using Testcontainers.Dynalite;

namespace Testcontainers.Dynalite;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class DynaliteBuilder : ContainerBuilder<DynaliteBuilder, DynaliteContainer, DynaliteConfiguration>
{
    public const string DynaliteImage = "quay.io/testcontainers/dynalite:v1.2.1-1";

    public const ushort DynalitePort = 4567;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynaliteBuilder" /> class.
    /// </summary>
    public DynaliteBuilder()
        : this(new DynaliteConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynaliteBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private DynaliteBuilder(DynaliteConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override DynaliteConfiguration DockerResourceConfiguration { get; }
    

    /// <inheritdoc />
    public override DynaliteContainer Build()
    {
        Validate();
        return new DynaliteContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override DynaliteBuilder Init()
    {
        return base.Init()
            .WithImage(DynaliteImage)
            .WithPortBinding(DynalitePort, true)
            .WithCommand("server", "/data")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request.ForPath("/minio/health/ready").ForPort(DynalitePort)));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();
    }

    /// <inheritdoc />
    protected override DynaliteBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DynaliteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DynaliteBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DynaliteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DynaliteBuilder Merge(DynaliteConfiguration oldValue, DynaliteConfiguration newValue)
    {
        return new DynaliteBuilder(new DynaliteConfiguration(oldValue, newValue));
    }
}