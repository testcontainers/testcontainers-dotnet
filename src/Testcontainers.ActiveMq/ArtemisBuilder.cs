namespace Testcontainers.ActiveMq;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ArtemisBuilder : ContainerBuilder<ArtemisBuilder, ArtemisContainer, ActiveMqConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string ArtemisImage = "apache/activemq-artemis:2.31.2";

    public const ushort ArtemisMainPort = 61616;

    public const ushort ArtemisConsolePort = 8161;

    public const string DefaultUsername = "artemis";

    public const string DefaultPassword = "artemis";

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtemisBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public ArtemisBuilder()
        : this(ArtemisImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtemisBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>apache/activemq-artemis:2.31.2</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/apache/activemq-artemis/tags" />.
    /// </remarks>
    public ArtemisBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtemisBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/apache/activemq-artemis/tags" />.
    /// </remarks>
    public ArtemisBuilder(IImage image)
        : this(new ActiveMqConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtemisBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ArtemisBuilder(ActiveMqConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ActiveMqConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Artemis username.
    /// </summary>
    /// <param name="username">The Artemis username.</param>
    /// <returns>A configured instance of <see cref="ArtemisBuilder" />.</returns>
    public ArtemisBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new ActiveMqConfiguration(username: username))
            .WithEnvironment("ARTEMIS_USER", username);
    }

    /// <summary>
    /// Sets the Artemis password.
    /// </summary>
    /// <param name="password">The Artemis password.</param>
    /// <returns>A configured instance of <see cref="ArtemisBuilder" />.</returns>
    public ArtemisBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new ActiveMqConfiguration(password: password))
            .WithEnvironment("ARTEMIS_PASSWORD", password);
    }

    /// <inheritdoc />
    public override ArtemisContainer Build()
    {
        Validate();
        return new ArtemisContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override ArtemisBuilder Init()
    {
        return base.Init()
            .WithPortBinding(ArtemisMainPort, true)
            .WithPortBinding(ArtemisConsolePort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("HTTP Server started"));
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
    protected override ArtemisBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ActiveMqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ArtemisBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ActiveMqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ArtemisBuilder Merge(ActiveMqConfiguration oldValue, ActiveMqConfiguration newValue)
    {
        return new ArtemisBuilder(new ActiveMqConfiguration(oldValue, newValue));
    }
}