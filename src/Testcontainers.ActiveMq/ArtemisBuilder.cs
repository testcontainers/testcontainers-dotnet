namespace Testcontainers.ActiveMq;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ArtemisBuilder : ContainerBuilder<ArtemisBuilder, ArtemisContainer, ActiveMqConfiguration>
{
    public const string ArtemisImage = "apache/activemq-artemis:2.31.2";

    public const ushort ArtemisMainPort = 61616;

    public const ushort ArtemisConsolePort = 8161;

    public const string DefaultUsername = "artemis";

    public const string DefaultPassword = "artemis";

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtemisBuilder" /> class.
    /// </summary>
    public ArtemisBuilder()
        : this(new ActiveMqConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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
        return new ArtemisContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override ArtemisBuilder Init()
    {
        return base.Init()
            .WithImage(ArtemisImage)
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