namespace Testcontainers.QuestDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class QuestDbBuilder : ContainerBuilder<QuestDbBuilder, QuestDbContainer, QuestDbConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string QuestDbImage = "questdb/questdb:10.0.1";

    public const ushort QuestDbPgPort = 8812;

    public const ushort QuestDbHttpPort = 9000;

    public const ushort QuestDbInfluxLinePort = 9009;

    public const string DefaultUsername = "quest";

    public const string DefaultPassword = "quest";

    public const string DefaultDatabase = "qdb";

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestDbBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public QuestDbBuilder()
        : this(QuestDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>questdb/questdb:10.0.1</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/questdb/questdb/tags" />.
    /// </remarks>
    public QuestDbBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/questdb/questdb/tags" />.
    /// </remarks>
    public QuestDbBuilder(IImage image)
        : this(new QuestDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private QuestDbBuilder(QuestDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override QuestDbConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the QuestDb username.
    /// </summary>
    /// <param name="username">The QuestDb username.</param>
    /// <returns>A configured instance of <see cref="QuestDbBuilder" />.</returns>
    public QuestDbBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new QuestDbConfiguration(username: username))
            .WithEnvironment("QDB_PG_USER", username);
    }

    /// <summary>
    /// Sets the QuestDb password.
    /// </summary>
    /// <param name="password">The QuestDb password.</param>
    /// <returns>A configured instance of <see cref="QuestDbBuilder" />.</returns>
    public QuestDbBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new QuestDbConfiguration(password: password))
            .WithEnvironment("QDB_PG_PASSWORD", password);
    }

    /// <inheritdoc />
    public override QuestDbContainer Build()
    {
        Validate();
        return new QuestDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override QuestDbBuilder Init()
    {
        return base.Init()
            .WithPortBinding(QuestDbPgPort, true)
            .WithPortBinding(QuestDbHttpPort, true)
            .WithPortBinding(QuestDbInfluxLinePort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithConnectionStringProvider(new QuestDbConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(QuestDbHttpPort)));
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
    protected override QuestDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new QuestDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override QuestDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new QuestDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override QuestDbBuilder Merge(QuestDbConfiguration oldValue, QuestDbConfiguration newValue)
    {
        return new QuestDbBuilder(new QuestDbConfiguration(oldValue, newValue));
    }
}