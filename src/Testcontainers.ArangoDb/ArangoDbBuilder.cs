namespace Testcontainers.ArangoDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ArangoDbBuilder : ContainerBuilder<ArangoDbBuilder, ArangoDbContainer, ArangoDbConfiguration>
{
    [Obsolete("This image tag is not recommended: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string ArangoDbImage = "arangodb:3.11.5";

    public const ushort ArangoDbPort = 8529;

    public const string DefaultUsername = "root";

    public const string DefaultPassword = "root";

    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public ArangoDbBuilder()
        : this(ArangoDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://hub.docker.com/_/arangodb/tags">https://hub.docker.com/_/arangodb/tags</see>.</param>
    public ArangoDbBuilder(string image)
        : this(new ArangoDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public ArangoDbBuilder(IImage image)
        : this(new ArangoDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ArangoDbBuilder(ArangoDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ArangoDbConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the ArangoDb password.
    /// </summary>
    /// <param name="password">The ArangoDb password.</param>
    /// <returns>A configured instance of <see cref="ArangoDbBuilder" />.</returns>
    public ArangoDbBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new ArangoDbConfiguration(password: password))
            .WithEnvironment("ARANGO_ROOT_PASSWORD", password);
    }

    /// <inheritdoc />
    public override ArangoDbContainer Build()
    {
        Validate();
        return new ArangoDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override ArangoDbBuilder Init()
    {
        return base.Init()
            .WithPortBinding(ArangoDbPort, true)
            .WithPassword(DefaultPassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Have fun!"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override ArangoDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ArangoDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ArangoDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ArangoDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ArangoDbBuilder Merge(ArangoDbConfiguration oldValue, ArangoDbConfiguration newValue)
    {
        return new ArangoDbBuilder(new ArangoDbConfiguration(oldValue, newValue));
    }
}