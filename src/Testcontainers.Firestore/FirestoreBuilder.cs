namespace Testcontainers.Firestore;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class FirestoreBuilder : ContainerBuilder<FirestoreBuilder, FirestoreContainer, FirestoreConfiguration>
{
    public const string GoogleCloudCliImage = "gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1-emulators";

    public const ushort FirestorePort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreBuilder" /> class.
    /// </summary>
    [Obsolete("Use constructor with image as a parameter instead.")]
    public FirestoreBuilder()
        : this(new FirestoreConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(GoogleCloudCliImage).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://console.cloud.google.com/artifacts/docker/google.com:cloudsdktool/us/gcr.io/google-cloud-cli?pli=1">https://console.cloud.google.com/artifacts/docker/google.com:cloudsdktool/us/gcr.io/google-cloud-cli?pli=1</see>.</param>
    public FirestoreBuilder(string image)
        : this(new FirestoreConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
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
            .WithImage(GoogleCloudCliImage)
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