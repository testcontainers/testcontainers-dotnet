namespace Testcontainers.Keycloak;

/// <inheritdoc />
[PublicAPI]
public sealed class KeycloakBuilder : ContainerBuilder<KeycloakBuilder, KeycloakContainer, KeycloakConfiguration>
{
    public const string KeycloakImage = "quay.io/keycloak/keycloak:21.1";

    public const ushort KeycloakPort = 8080;

    public const ushort KeycloakHealthPort = 9000;

    public const string DefaultUsername = "admin";

    public const string DefaultPassword = "admin";

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakBuilder" /> class.
    /// </summary>
    public KeycloakBuilder()
        : this(new KeycloakConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private KeycloakBuilder(KeycloakConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override KeycloakConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Keycloak admin username.
    /// </summary>
    /// <param name="username">The Keycloak admin username.</param>
    /// <returns>A configured instance of <see cref="KeycloakBuilder" />.</returns>
    public KeycloakBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new KeycloakConfiguration(username: username))
            .WithEnvironment("KEYCLOAK_ADMIN", username);
    }

    /// <summary>
    /// Sets the Keycloak admin password.
    /// </summary>
    /// <param name="password">The Keycloak admin password.</param>
    /// <returns>A configured instance of <see cref="KeycloakBuilder" />.</returns>
    public KeycloakBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new KeycloakConfiguration(password: password))
            .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", password);
    }

    /// <inheritdoc />
    public override KeycloakContainer Build()
    {
        Validate();

        var image = DockerResourceConfiguration.Image;
        _ = image.MatchVersion((string tag) => "Foo".Equals(tag));

        var majorVersionString = new string(DockerResourceConfiguration.Image.Tag.TakeWhile(char.IsDigit).ToArray());

        if (!isLatestVersion)
        {
            var majorVersionString = new string(DockerResourceConfiguration.Image.Tag.TakeWhile(char.IsDigit).ToArray());

            // https://www.keycloak.org/docs/latest/release_notes/index.html#management-port-for-metrics-and-health-endpoints.
            isLatestVersion |= (int.TryParse(majorVersionString, out var majorVersion) && majorVersion >= 25);
        }

        var waitStrategy = Wait.ForUnixContainer()
            .UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/health/ready").ForPort(isLatestVersion ? KeycloakHealthPort : KeycloakPort));

        var keycloakBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(waitStrategy);
        return new KeycloakContainer(keycloakBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override KeycloakBuilder Init()
    {
        return base.Init()
            .WithImage(KeycloakImage)
            .WithCommand("start-dev")
            .WithPortBinding(KeycloakPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithEnvironment("KC_HEALTH_ENABLED", "true")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request.ForPath("/health/ready").ForPort(KeycloakPort)));
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
    protected override KeycloakBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KeycloakConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KeycloakBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KeycloakConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KeycloakBuilder Merge(KeycloakConfiguration oldValue, KeycloakConfiguration newValue)
    {
        return new KeycloakBuilder(new KeycloakConfiguration(oldValue, newValue));
    }
}