namespace Testcontainers.Bigtable;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class BigtableBuilder : ContainerBuilder<BigtableBuilder, BigtableContainer, BigtableConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string GoogleCloudCliImage = "gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators";

    public const ushort BigtablePort = 9000;

    /// <summary>
    /// Initializes a new instance of the <see cref="BigtableBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public BigtableBuilder()
        : this(GoogleCloudCliImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigtableBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://console.cloud.google.com/artifacts/docker/google.com:cloudsdktool/us/gcr.io/google-cloud-cli" />.
    /// </remarks>
    public BigtableBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigtableBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://console.cloud.google.com/artifacts/docker/google.com:cloudsdktool/us/gcr.io/google-cloud-cli" />.
    /// </remarks>
    public BigtableBuilder(IImage image)
        : this(new BigtableConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigtableBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private BigtableBuilder(BigtableConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override BigtableConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override BigtableContainer Build()
    {
        Validate();
        return new BigtableContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override BigtableBuilder Init()
    {
        return base.Init()
            .WithPortBinding(BigtablePort, true)
            .WithEntrypoint("gcloud")
            .WithCommand("beta", "emulators", "bigtable", "start", "--host-port", "0.0.0.0:" + BigtablePort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*running.*$"));
    }

    /// <inheritdoc />
    protected override BigtableBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new BigtableConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override BigtableBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new BigtableConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override BigtableBuilder Merge(BigtableConfiguration oldValue, BigtableConfiguration newValue)
    {
        return new BigtableBuilder(new BigtableConfiguration(oldValue, newValue));
    }
}