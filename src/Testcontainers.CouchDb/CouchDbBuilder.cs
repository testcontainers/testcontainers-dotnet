namespace Testcontainers.CouchDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class CouchDbBuilder : ContainerBuilder<CouchDbBuilder, CouchDbContainer, CouchDbConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string CouchDbImage = "couchdb:3.3";

    public const ushort CouchDbPort = 5984;

    public const string DefaultUsername = "couchdb";

    public const string DefaultPassword = "couchdb";

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchDbBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public CouchDbBuilder()
        : this(CouchDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>couchdb:3.3</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/couchdb/tags" />.
    /// </remarks>
    public CouchDbBuilder(string image)
        : this(new CouchDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/couchdb/tags" />.
    /// </remarks>
    public CouchDbBuilder(IImage image)
        : this(new CouchDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private CouchDbBuilder(CouchDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override CouchDbConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the CouchDb username.
    /// </summary>
    /// <param name="username">The CouchDb username.</param>
    /// <returns>A configured instance of <see cref="CouchDbBuilder" />.</returns>
    public CouchDbBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new CouchDbConfiguration(username: username))
            .WithEnvironment("COUCHDB_USER", username);
    }

    /// <summary>
    /// Sets the CouchDb password.
    /// </summary>
    /// <param name="password">The CouchDb password.</param>
    /// <returns>A configured instance of <see cref="CouchDbBuilder" />.</returns>
    public CouchDbBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new CouchDbConfiguration(password: password))
            .WithEnvironment("COUCHDB_PASSWORD", password);
    }

    /// <inheritdoc />
    public override CouchDbContainer Build()
    {
        Validate();
        return new CouchDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override CouchDbBuilder Init()
    {
        return base.Init()
            .WithPortBinding(CouchDbPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(CouchDbPort)));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Username, nameof(DockerResourceConfiguration.Username))
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override CouchDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CouchDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CouchDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CouchDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CouchDbBuilder Merge(CouchDbConfiguration oldValue, CouchDbConfiguration newValue)
    {
        return new CouchDbBuilder(new CouchDbConfiguration(oldValue, newValue));
    }
}