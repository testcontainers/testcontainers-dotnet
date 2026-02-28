namespace Testcontainers.KurrentDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class KurrentDbBuilder : ContainerBuilder<KurrentDbBuilder, KurrentDbContainer, KurrentDbConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string KurrentDbImage = "kurrentplatform/kurrentdb:25.1";

    public const ushort KurrentDbPort = 2113;

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public KurrentDbBuilder()
        : this(KurrentDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>kurrentplatform/kurrentdb:25.1</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/kurrentplatform/kurrentdb/tags" />.
    /// </remarks>
    public KurrentDbBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/kurrentplatform/kurrentdb/tags" />.
    /// </remarks>
    public KurrentDbBuilder(IImage image)
        : this(new KurrentDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private KurrentDbBuilder(KurrentDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override KurrentDbConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override KurrentDbContainer Build()
    {
        return new KurrentDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override KurrentDbBuilder Init()
    {
        return base.Init()
            .WithPortBinding(KurrentDbPort, true)
            .WithEnvironment("KURRENTDB_CLUSTER_SIZE", "1")
            .WithEnvironment("KURRENTDB_RUN_PROJECTIONS", "All")
            .WithEnvironment("KURRENTDB_START_STANDARD_PROJECTIONS", "true")
            .WithEnvironment("KURRENTDB_INSECURE", "true")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy());
    }

    /// <inheritdoc />
    protected override KurrentDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KurrentDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KurrentDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KurrentDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KurrentDbBuilder Merge(KurrentDbConfiguration oldValue, KurrentDbConfiguration newValue)
    {
        return new KurrentDbBuilder(new KurrentDbConfiguration(oldValue, newValue));
    }
}