namespace Testcontainers.QuestDb;

using System.Net;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class QuestDbBuilder : ContainerBuilder<QuestDbBuilder, QuestDbContainer, QuestDbConfiguration>
{
    public const ushort QuestDbPgPort = 8812;

    public const ushort QuestDbHttpPort = 9000;

    public const ushort QuestDbInfluxLinePort = 9009;

    public const string DefaultUsername = "admin";

    public const string DefaultPassword = "quest";

    public const string DefaultDatabase = "qdb";

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>questdb/questdb:9.2.3</c>).
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
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r
                    .ForPort(QuestDbHttpPort)
                    .ForPath("/")
                    .ForStatusCode(HttpStatusCode.OK)));
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