namespace Testcontainers.CockroachDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class CockroachDbBuilder : ContainerBuilder<CockroachDbBuilder, CockroachDbContainer, CockroachDbConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string CockroachDbImage = "cockroachdb/cockroach:latest-v23.1";

    public const ushort CockroachDbPort = 26257;

    public const ushort CockroachDbRestPort = 8080;

    public const string DefaultDatabase = "defaultdb";

    public const string DefaultUsername = "root";

    public const string DefaultPassword = "";

    /// <summary>
    /// Initializes a new instance of the <see cref="CockroachDbBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public CockroachDbBuilder()
        : this(CockroachDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CockroachDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>cockroachdb/cockroach:latest-v23.1</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/cockroachdb/cockroach/tags" />.
    /// </remarks>
    public CockroachDbBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CockroachDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/cockroachdb/cockroach/tags" />.
    /// </remarks>
    public CockroachDbBuilder(IImage image)
        : this(new CockroachDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CockroachDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private CockroachDbBuilder(CockroachDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override CockroachDbConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the CockroachDb database.
    /// </summary>
    /// <param name="database">The CockroachDb database.</param>
    /// <returns>A configured instance of <see cref="CockroachDbBuilder" />.</returns>
    public CockroachDbBuilder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new CockroachDbConfiguration(database: database))
            .WithEnvironment("COCKROACH_DATABASE", database);
    }

    /// <summary>
    /// Sets the CockroachDb username.
    /// </summary>
    /// <param name="username">The CockroachDb username.</param>
    /// <returns>A configured instance of <see cref="CockroachDbBuilder" />.</returns>
    public CockroachDbBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new CockroachDbConfiguration(username: username))
            .WithEnvironment("COCKROACH_USER", username);
    }

    /// <summary>
    /// Sets the CockroachDb password.
    /// </summary>
    /// <param name="password">The CockroachDb password.</param>
    /// <returns>A configured instance of <see cref="CockroachDbBuilder" />.</returns>
    public CockroachDbBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new CockroachDbConfiguration(password: password))
            .WithEnvironment("COCKROACH_PASSWORD", password);
    }

    /// <inheritdoc />
    public override CockroachDbContainer Build()
    {
        Validate();
        return new CockroachDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override CockroachDbBuilder Init()
    {
        return base.Init()
            .WithPortBinding(CockroachDbPort, true)
            .WithPortBinding(CockroachDbRestPort, true)
            .WithDatabase(DefaultDatabase)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithCommand("start-single-node")
            .WithCommand("--insecure")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPort(CockroachDbRestPort).ForPath("/health")));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull();
    }

    /// <inheritdoc />
    protected override CockroachDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CockroachDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CockroachDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CockroachDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CockroachDbBuilder Merge(CockroachDbConfiguration oldValue, CockroachDbConfiguration newValue)
    {
        return new CockroachDbBuilder(new CockroachDbConfiguration(oldValue, newValue));
    }
}