namespace Testcontainers.OpenSearch;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OpenSearchBuilder : ContainerBuilder<OpenSearchBuilder, OpenSearchContainer, OpenSearchConfiguration>
{
    public const string DefaultUsername = "admin";

    public const string DefaultPassword = "VeryStrongP@ssw0rd!";

    public const string DefaultOldInsecurePassword = "admin";

    public const string OpenSearchImage = "opensearchproject/opensearch:2.12.0";

    // Default HTTP port.
    public const ushort OpenSearchHttpApiPort = 9200;

    // Default TCP port (deprecated and may be removed in future versions).
    public const ushort OpenSearchTcpPort = 9300;

    public const ushort OpenSearchPerfAnalyzerPort = 9600;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchBuilder" /> class.
    /// </summary>
    public OpenSearchBuilder()
        : this(new OpenSearchConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private OpenSearchBuilder(OpenSearchConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    protected override OpenSearchConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the password for 'admin' user.
    /// </summary>
    /// <param name="password">Password requires a minimum of 8 characters and must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.</param>
    /// <returns>A configured instance of <see cref="OpenSearchBuilder" />.</returns>
    public OpenSearchBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(password: password))
            .WithEnvironment("OPENSEARCH_INITIAL_ADMIN_PASSWORD", password);
    }

    /// <summary>
    /// Disables build-in security plugin.
    /// Connections returned by container's GetConnection() method will also use 'http' protocol instead of 'https'.
    /// </summary>
    /// <param name="disabled">'True' for disabling security pligin. Default value is 'true'.</param>
    /// <returns></returns>
    public OpenSearchBuilder WithDisabledSecurity(bool disabled = true)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(disabledSecurity: disabled))
            .WithEnvironment("plugins.security.disabled", disabled.ToString().ToLowerInvariant())
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilHttpRequestIsSucceeded(
                    r => r
                        .UsingTls(!disabled)
                        .UsingHttpMessageHandler(new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                        })
                        .WithBasicAuthentication(DefaultUsername, DockerResourceConfiguration.Password ?? DefaultPassword)
                        .ForPort(OpenSearchHttpApiPort)
                        .ForStatusCodeMatching(s => s == HttpStatusCode.OK || s == HttpStatusCode.Unauthorized)
                )
            );
    }

    /// <inheritdoc />
    public override OpenSearchContainer Build()
    {
        Validate();
        return new OpenSearchContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Init()
    {
        return base.Init()
            .WithImage(OpenSearchImage)
            .WithPortBinding(OpenSearchTcpPort, true)
            .WithPortBinding(OpenSearchHttpApiPort, true)
            .WithPortBinding(OpenSearchPerfAnalyzerPort, true)
            .WithEnvironment("discovery.type", "single-node")
            .WithPassword(DefaultPassword)
            .WithDisabledSecurity(false);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Merge(OpenSearchConfiguration oldValue, OpenSearchConfiguration newValue)
    {
        return new OpenSearchBuilder(new OpenSearchConfiguration(oldValue, newValue));
    }
}
