namespace Testcontainers.Grafana;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class GrafanaBuilder : ContainerBuilder<GrafanaBuilder, GrafanaContainer, GrafanaConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string GrafanaImage = "grafana/grafana:12.2";

    public const ushort GrafanaPort = 3000;

    public const string DefaultUsername = "admin";

    public const string DefaultPassword = "admin";

    /// <summary>
    /// Initializes a new instance of the <see cref="GrafanaBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public GrafanaBuilder()
        : this(GrafanaImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrafanaBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>grafana/grafana:12.2</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/grafana/grafana/tags" />.
    /// </remarks>
    public GrafanaBuilder(string image)
        : this(new GrafanaConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrafanaBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/grafana/grafana/tags" />.
    /// </remarks>
    public GrafanaBuilder(IImage image)
        : this(new GrafanaConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrafanaBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private GrafanaBuilder(GrafanaConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override GrafanaConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Grafana username.
    /// </summary>
    /// <param name="username">The Grafana username.</param>
    /// <returns>A configured instance of <see cref="GrafanaBuilder" />.</returns>
    public GrafanaBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new GrafanaConfiguration(username: username))
            .WithEnvironment("GF_SECURITY_ADMIN_USER", username);
    }

    /// <summary>
    /// Sets the Grafana password.
    /// </summary>
    /// <param name="password">The Grafana password.</param>
    /// <returns>A configured instance of <see cref="GrafanaBuilder" />.</returns>
    public GrafanaBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new GrafanaConfiguration(password: password))
            .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", password);
    }

    /// <summary>
    /// Enables the anonymous access.
    /// </summary>
    /// <returns>A configured instance of <see cref="GrafanaBuilder" />.</returns>
    public GrafanaBuilder WithAnonymousAccessEnabled()
    {
        return WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "true")
            .WithEnvironment("GF_AUTH_ANONYMOUS_ORG_ROLE", "Admin");
    }

    /// <inheritdoc />
    public override GrafanaContainer Build()
    {
        Validate();
        return new GrafanaContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override GrafanaBuilder Init()
    {
        return base.Init()
            .WithPortBinding(GrafanaPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/api/health").ForPort(GrafanaPort)));
    }

    /// <inheritdoc />
    protected override GrafanaBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new GrafanaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override GrafanaBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new GrafanaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override GrafanaBuilder Merge(GrafanaConfiguration oldValue, GrafanaConfiguration newValue)
    {
        return new GrafanaBuilder(new GrafanaConfiguration(oldValue, newValue));
    }
}