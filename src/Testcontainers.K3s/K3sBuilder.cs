namespace Testcontainers.K3s;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class K3sBuilder : ContainerBuilder<K3sBuilder, K3sContainer, K3sConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string RancherImage = "rancher/k3s:v1.26.2-k3s1";

    public const ushort KubeSecurePort = 6443;

    public const ushort RancherWebhookPort = 8443;

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public K3sBuilder()
        : this(RancherImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>rancher/k3s:v1.26.2-k3s1</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/rancher/k3s/tags" />.
    /// </remarks>
    public K3sBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/rancher/k3s/tags" />.
    /// </remarks>
    public K3sBuilder(IImage image)
        : this(new K3sConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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
        return new K3sContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override K3sBuilder Init()
    {
        return base.Init()
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