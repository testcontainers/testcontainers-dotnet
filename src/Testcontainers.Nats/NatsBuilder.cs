namespace Testcontainers.Nats;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class NatsBuilder : ContainerBuilder<NatsBuilder, NatsContainer, NatsConfiguration>
{
    public const string NatsImage = "nats:2.9";

    public const ushort NatsClientPort = 4222;

    public const ushort NatsClusterRoutingPort = 6222;

    public const ushort NatsHttpManagementPort = 8222;

    /// <summary>
    /// Initializes a new instance of the <see cref="NatsBuilder" /> class.
    /// </summary>
    public NatsBuilder()
        : this(new NatsConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NatsBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private NatsBuilder(NatsConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override NatsConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Nats username.
    /// </summary>
    /// <param name="username">The Nats username.</param>
    /// <returns>A configured instance of <see cref="NatsBuilder" />.</returns>
    public NatsBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new NatsConfiguration(username: username))
            .WithCommand("--user", username);
    }

    /// <summary>
    /// Sets the Nats password.
    /// </summary>
    /// <param name="password">The Nats password.</param>
    /// <returns>A configured instance of <see cref="NatsBuilder" />.</returns>
    public NatsBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new NatsConfiguration(password: password))
            .WithCommand("--pass", password);
    }

    /// <inheritdoc />
    public override NatsContainer Build()
    {
        Validate();
        return new NatsContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override NatsBuilder Init()
    {
        return base.Init()
            .WithImage(NatsImage)
            .WithPortBinding(NatsClientPort, true)
            .WithPortBinding(NatsHttpManagementPort, true)
            .WithPortBinding(NatsClusterRoutingPort, true)
            .WithUsername(string.Empty)
            .WithPassword(string.Empty)
            .WithCommand("--http_port", NatsHttpManagementPort.ToString())
            .WithCommand("--jetstream")
            .WithCommand("--debug")
            .WithCommand("--trace")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server is ready"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        const string message = "Missing username or password. Both must be specified.";

        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Username, nameof(DockerResourceConfiguration.Username))
            .NotNull();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull();

        _ = Guard.Argument(DockerResourceConfiguration, "Credentials")
            .ThrowIf(argument => 1.Equals(new[] { argument.Value.Username, argument.Value.Password }.Count(string.IsNullOrWhiteSpace)), argument => new ArgumentException(message, argument.Name));
    }

    /// <inheritdoc />
    protected override NatsBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new NatsConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override NatsBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new NatsConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override NatsBuilder Merge(NatsConfiguration oldValue, NatsConfiguration newValue)
    {
        return new NatsBuilder(new NatsConfiguration(oldValue, newValue));
    }
}