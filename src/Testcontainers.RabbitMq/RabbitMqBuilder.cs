namespace Testcontainers.RabbitMq;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class RabbitMqBuilder : ContainerBuilder<RabbitMqBuilder, RabbitMqContainer, RabbitMqConfiguration>
{
    public const string RabbitMqImage = "rabbitmq:3.11";

    public const ushort RabbitMqPort = 5672;

    public const string DefaultUsername = "rabbitmq";

    public const string DefaultPassword = "rabbitmq";

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqBuilder" /> class.
    /// </summary>
    public RabbitMqBuilder()
        : this(new RabbitMqConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private RabbitMqBuilder(RabbitMqConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override RabbitMqConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the RabbitMq username.
    /// </summary>
    /// <param name="username">The RabbitMq username.</param>
    /// <returns>A configured instance of <see cref="RabbitMqBuilder" />.</returns>
    public RabbitMqBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new RabbitMqConfiguration(username: username))
            .WithEnvironment("RABBITMQ_DEFAULT_USER", username);
    }

    /// <summary>
    /// Sets the RabbitMq password.
    /// </summary>
    /// <param name="password">The RabbitMq password.</param>
    /// <returns>A configured instance of <see cref="RabbitMqBuilder" />.</returns>
    public RabbitMqBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new RabbitMqConfiguration(password: password))
            .WithEnvironment("RABBITMQ_DEFAULT_PASS", password);
    }

    /// <inheritdoc />
    public override RabbitMqContainer Build()
    {
        Validate();
        return new RabbitMqContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override RabbitMqBuilder Init()
    {
        return base.Init()
            .WithImage(RabbitMqImage)
            .WithPortBinding(RabbitMqPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server startup complete"));
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
    protected override RabbitMqBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RabbitMqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RabbitMqBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RabbitMqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RabbitMqBuilder Merge(RabbitMqConfiguration oldValue, RabbitMqConfiguration newValue)
    {
        return new RabbitMqBuilder(new RabbitMqConfiguration(oldValue, newValue));
    }
}