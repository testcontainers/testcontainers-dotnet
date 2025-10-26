namespace Testcontainers.Grafana;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class GrafanaBuilder : ContainerBuilder<GrafanaBuilder, GrafanaContainer, GrafanaConfiguration>
{
    public const string GrafanaImage = "grafana/grafana:11.0.0";

    public const ushort GrafanaPort = 3000;

    public const string DefaultUsername = "admin";

    public const string DefaultPassword = "admin";

    /// <summary>
    /// Initializes a new instance of the <see cref="GrafanaBuilder" /> class.
    /// </summary>
    public GrafanaBuilder()
        : this(new GrafanaConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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
    /// Disables the anonymous access.
    /// </summary>
    /// <returns>A configured instance of <see cref="GrafanaBuilder" />.</returns>
    public GrafanaBuilder WithAnonymousAccessDisabled()
    {
        return WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "false");
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

    /// <summary>
    /// Mounts a datasource configuration file.
    /// </summary>
    /// <param name="datasourceFilePath">The path to the datasource configuration file.</param>
    /// <returns>A configured instance of <see cref="GrafanaBuilder" />.</returns>
    public GrafanaBuilder WithDataSource(string datasourceFilePath)
    {
        return WithBindMount(datasourceFilePath, "/etc/grafana/provisioning/datasources/");
    }

    /// <summary>
    /// Mounts a dashboard configuration file.
    /// </summary>
    /// <param name="dashboardFilePath">The path to the dashboard configuration file.</param>
    /// <returns>A configured instance of <see cref="GrafanaBuilder" />.</returns>
    public GrafanaBuilder WithDashboard(string dashboardFilePath)
    {
        return WithBindMount(dashboardFilePath, "/etc/grafana/provisioning/dashboards/");
    }

    /// <summary>
    /// Mounts a plugin configuration file.
    /// </summary>
    /// <param name="pluginFilePath">The path to the plugin configuration file.</param>
    /// <returns>A configured instance of <see cref="GrafanaBuilder" />.</returns>
    public GrafanaBuilder WithPlugin(string pluginFilePath)
    {
        return WithBindMount(pluginFilePath, "/etc/grafana/provisioning/plugins/");
    }

    /// <summary>
    /// Mounts a notifier configuration file.
    /// </summary>
    /// <param name="notifierFilePath">The path to the notifier configuration file.</param>
    /// <returns>A configured instance of <see cref="GrafanaBuilder" />.</returns>
    public GrafanaBuilder WithNotifier(string notifierFilePath)
    {
        return WithBindMount(notifierFilePath, "/etc/grafana/provisioning/notifiers/");
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
            .WithImage(GrafanaImage)
            .WithPortBinding(GrafanaPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "false")
            .WithEnvironment("GF_AUTH_BASIC_ENABLED", "true")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request
                    .ForPort(GrafanaPort)
                    .ForPath("/api/health")
                    .ForStatusCode(HttpStatusCode.OK)));
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