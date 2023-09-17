using DotNet.Testcontainers;

namespace Testcontainers.Nats;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class NatsBuilder : ContainerBuilder<NatsBuilder, NatsContainer, NatsConfiguration>
{
    public const string NatsImage = "nats:2.9";

    public const ushort ClientPort = 4222;
    public const ushort RoutingPort = 6222;
    public const ushort MonitoringPort = 8222;

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
    /// Sets the Nats Server password.
    /// </summary>
    /// <param name="password">The Nats Server password.</param>
    /// <returns>A configured instance of <see cref="NatsBuilder" />.</returns>
    public NatsBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new NatsConfiguration(password: password))
            .WithCommand("--pass", password);
    }

    /// <summary>
    /// Sets the Nats Server username.
    /// </summary>
    /// <param name="username">The Nats Server username.</param>
    /// <returns>A configured instance of <see cref="NatsBuilder" />.</returns>
    public NatsBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new NatsConfiguration(username: username))
            .WithCommand("--user", username);
    }

    /// <inheritdoc />
    public override NatsContainer Build()
    {
        Validate();
        return new NatsContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    
    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();
        
        if (DockerResourceConfiguration.Password != null || DockerResourceConfiguration.Username != null)
        {
            _ = Guard.Argument(DockerResourceConfiguration.Username, nameof(DockerResourceConfiguration.Username))
                .NotNull();

            _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
                .NotNull();
        }
    }

    /// <inheritdoc />
    protected override NatsBuilder Init()
    {
        return base.Init()
            .WithImage(NatsImage)
            .WithPortBinding(ClientPort, true)
            .WithPortBinding(MonitoringPort, true)
            .WithPortBinding(RoutingPort, true)
            .WithCommand("--http_port", MonitoringPort.ToString()) // Enable monitoring endpoint.
            .WithCommand("--jetstream") // Enable JetStream functionality.
            .WithCommand("--debug") // Enable both debug 
            .WithCommand("--trace") // Enable protocol trace messages 
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("Listening for client connections on 0.0.0.0:4222"));
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