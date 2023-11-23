namespace Testcontainers.ActiveMq;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ActiveMqBuilder : ContainerBuilder<ActiveMqBuilder, ActiveMqContainer, ActiveMqConfiguration>
{
    public const string ActiveMqImage = "apache/activemq-artemis:2.31.2";
    public const ushort ActiveMqMainPort = 61616;
    public const ushort ActiveMqConsolePort = 8161;
    public const string DefaultUsername = "artemis";
    public const string DefaultPassword = "artemis";

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveMqBuilder" /> class.
    /// </summary>
    public ActiveMqBuilder()
        : this(new ActiveMqConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveMqBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ActiveMqBuilder(ActiveMqConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    // <inheritdoc />
    protected override ActiveMqConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the ActiveMq username.
    /// </summary>
    /// <param name="username">The ActiveMq username.</param>
    /// <returns>A configured instance of <see cref="ActiveMqBuilder" />.</returns>
    public ActiveMqBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new ActiveMqConfiguration(username: username))
            .WithEnvironment("ARTEMIS_USER", username);
    }

    /// <summary>
    /// Sets the ActiveMq password.
    /// </summary>
    /// <param name="password">The ActiveMq password.</param>
    /// <returns>A configured instance of <see cref="ActiveMqBuilder" />.</returns>
    public ActiveMqBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new ActiveMqConfiguration(password: password))
            .WithEnvironment("ARTEMIS_PASSWORD", password);
    }

    /// <inheritdoc />
    public override ActiveMqContainer Build()
    {
        Validate();
        return new ActiveMqContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override ActiveMqBuilder Init()
    {
        return base.Init()
            .WithImage(ActiveMqImage)
            .WithPortBinding(ActiveMqMainPort, true)
            .WithPortBinding(ActiveMqConsolePort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()));
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
    protected override ActiveMqBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ActiveMqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ActiveMqBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ActiveMqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ActiveMqBuilder Merge(ActiveMqConfiguration oldValue, ActiveMqConfiguration newValue)
    {
        return new ActiveMqBuilder(new ActiveMqConfiguration(oldValue, newValue));
    }

    private sealed class WaitUntil : IWaitUntil
    {
        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var (stdout, _) = await container.GetLogsAsync(timestampsEnabled: false)
                .ConfigureAwait(false);

            return stdout.Contains("HTTP Server started");
        }
    }
}