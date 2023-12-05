namespace Testcontainers.K3s;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class K3sBuilder : ContainerBuilder<K3sBuilder, K3sContainer, K3sConfiguration>
{
    public const string RancherImage = "rancher/k3s:v1.26.2-k3s1";

    public const ushort KubeSecurePort = 6443;

    public const ushort RancherWebhookPort = 8443;

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sBuilder" /> class.
    /// </summary>
    public K3sBuilder()
        : this(new K3sConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private K3sBuilder(K3sConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override K3sConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override K3sContainer Build()
    {
        Validate();
        return new K3sContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override K3sBuilder Init()
    {
        return base.Init()
            .WithImage(RancherImage)
            .WithPrivileged(true)
            .WithPortBinding(KubeSecurePort, true)
            .WithPortBinding(RancherWebhookPort, true)
            .WithBindMount("/sys/fs/cgroup", "/sys/fs/cgroup", AccessMode.ReadWrite)
            .WithTmpfsMount("/run")
            .WithTmpfsMount("/var/run")
            .WithCommand("server", "--disable=traefik")
            .WithCreateParameterModifier(parameterModifier => parameterModifier.HostConfig.CgroupnsMode = "host")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Node controller sync successful"));
    }

    /// <inheritdoc />
    protected override K3sBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new K3sConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override K3sBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new K3sConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override K3sBuilder Merge(K3sConfiguration oldValue, K3sConfiguration newValue)
    {
        return new K3sBuilder(new K3sConfiguration(oldValue, newValue));
    }
}