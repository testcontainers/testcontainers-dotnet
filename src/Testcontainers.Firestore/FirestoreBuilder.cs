namespace Testcontainers.Firestore;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class FirestoreBuilder : ContainerBuilder<FirestoreBuilder, FirestoreContainer, FirestoreConfiguration>
{
    const string Command = "gcloud beta emulators firestore start --host-port 0.0.0.0:8080";
    const string Image = "gcr.io/google.com/cloudsdktool/google-cloud-cli:441.0.0-emulators";

    /// <summary>
    /// Initializes a new instance of the <see cref="FirestoreBuilder" /> class.
    /// </summary>
    public FirestoreBuilder()
        : this(new FirestoreConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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

    protected override FirestoreConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override FirestoreContainer Build()
    {
        Validate();
        return new FirestoreContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
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


    /// <inheritdoc />
    protected override FirestoreBuilder Init()
    {
        return base.Init()
          .WithImage(Image)
          .WithPortBinding(8080)
          .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("running"))
          .WithCommand("/bin/sh", "-c", Command)          
          ;
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();
    }

}