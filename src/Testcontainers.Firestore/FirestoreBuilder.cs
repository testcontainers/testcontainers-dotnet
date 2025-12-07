namespace Testcontainers.Firestore;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class FirestoreBuilder : ContainerBuilder<FirestoreBuilder, FirestoreContainer, FirestoreConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string GoogleCloudCliImage = "gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators";

    public const ushort FirestorePort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public FirestoreBuilder()
        : this(GoogleCloudCliImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://console.cloud.google.com/artifacts/docker/google.com:cloudsdktool/us/gcr.io/google-cloud-cli" />.
    /// </remarks>
    public FirestoreBuilder(string image)
        : this(new FirestoreConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://console.cloud.google.com/artifacts/docker/google.com:cloudsdktool/us/gcr.io/google-cloud-cli" />.
    /// </remarks>
    public FirestoreBuilder(IImage image)
        : this(new FirestoreConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private FirestoreBuilder(FirestoreConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override FirestoreConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override FirestoreContainer Build()
    {
        Validate();
        return new FirestoreContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override FirestoreBuilder Init()
    {
        return base.Init()
            .WithPortBinding(FirestorePort, true)
            .WithEntrypoint("gcloud")
            .WithCommand("beta", "emulators", "firestore", "start", "--host-port", "0.0.0.0:" + FirestorePort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*running.*$"));
    }

    /// <inheritdoc />
    protected override FirestoreBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FirestoreConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FirestoreBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FirestoreConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FirestoreBuilder Merge(FirestoreConfiguration oldValue, FirestoreConfiguration newValue)
    {
        return new FirestoreBuilder(new FirestoreConfiguration(oldValue, newValue));
    }
}