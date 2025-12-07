namespace Testcontainers.OpenSearch;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OpenSearchBuilder : ContainerBuilder<OpenSearchBuilder, OpenSearchContainer, OpenSearchConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string OpenSearchImage = "opensearchproject/opensearch:2.12.0";

    public const ushort OpenSearchRestApiPort = 9200;

    public const ushort OpenSearchTransportPort = 9300;

    public const ushort OpenSearchPerformanceAnalyzerPort = 9600;

    public const string DefaultUsername = "admin";

    public const string DefaultPassword = "yourStrong(!)P@ssw0rd";

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public OpenSearchBuilder()
        : this(OpenSearchImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>opensearchproject/opensearch:2.12.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/opensearchproject/opensearch/tags" />.
    /// </remarks>
    public OpenSearchBuilder(string image)
        : this(new OpenSearchConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/opensearchproject/opensearch/tags" />.
    /// </remarks>
    public OpenSearchBuilder(IImage image)
        : this(new OpenSearchConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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

    /// <inheritdoc />
    protected override OpenSearchConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the password for the <c>admin</c> user.
    /// </summary>
    /// <remarks>
    /// The password must meet the following complexity requirements:
    /// <list type="bullet">
    ///     <item><description>Minimum of 8 characters</description></item>
    ///     <item><description>At least one uppercase letter</description></item>
    ///     <item><description>At least one lowercase letter</description></item>
    ///     <item><description>At least one digit</description></item>
    ///     <item><description>At least one special character</description></item>
    /// </list>
    /// </remarks>
    /// <param name="password">The <c>admin</c> user password.</param>
    /// <returns>A configured instance of <see cref="OpenSearchBuilder" />.</returns>
    public OpenSearchBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(password: password))
            .WithEnvironment("OPENSEARCH_INITIAL_ADMIN_PASSWORD", password);
    }

    /// <summary>
    /// Enables or disables the built-in security plugin in OpenSearch.
    /// </summary>
    /// <remarks>
    /// When disabled, the <see cref="OpenSearchContainer.GetConnectionString" /> method
    /// will use the <c>http</c> protocol instead of <c>https</c>.
    /// </remarks>
    /// <param name="securityEnabled"><c>true</c> to enable the security plugin; <c>false</c> to disable it.</param>
    /// <returns>A configured instance of <see cref="OpenSearchBuilder" />.</returns>
    public OpenSearchBuilder WithSecurityEnabled(bool securityEnabled = true)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(tlsEnabled: securityEnabled))
            .WithEnvironment("plugins.security.disabled", (!securityEnabled).ToString().ToLowerInvariant());
    }

    /// <inheritdoc />
    public override OpenSearchContainer Build()
    {
        Validate();

        OpenSearchBuilder openSearchBuilder;

        Predicate<System.Version> predicate = v => v.Major == 1 || (v.Major == 2 && v.Minor < 12);

        var image = DockerResourceConfiguration.Image;

        // Images before version 2.12.0 use a hardcoded default password.
        var requiresHardcodedDefaultPassword = image.MatchVersion(predicate);
        if (requiresHardcodedDefaultPassword)
        {
            openSearchBuilder = WithPassword("admin");
        }
        else
        {
            openSearchBuilder = this;
        }

        // By default, the base builder waits until the container is running. However, for OpenSearch, a more advanced waiting strategy is necessary that requires access to the password.
        // If the user does not provide a custom waiting strategy, append the default OpenSearch waiting strategy.
        openSearchBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? openSearchBuilder : openSearchBuilder.WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil(DockerResourceConfiguration)));
        return new OpenSearchContainer(openSearchBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Init()
    {
        return base.Init()
            .WithPortBinding(OpenSearchRestApiPort, true)
            .WithPortBinding(OpenSearchTransportPort, true)
            .WithPortBinding(OpenSearchPerformanceAnalyzerPort, true)
            .WithEnvironment("discovery.type", "single-node")
            .WithSecurityEnabled()
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword);
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

    /// <summary>
    /// Sets the OpenSearch username.
    /// </summary>
    /// <remarks>
    /// The Docker image does not allow to configure the username.
    /// </remarks>
    /// <param name="username">The OpenSearch username.</param>
    /// <returns>A configured instance of <see cref="OpenSearchBuilder" />.</returns>
    private OpenSearchBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(username: username));
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        private readonly bool _tlsEnabled;

        private readonly string _username;

        private readonly string _password;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntil" /> class.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        public WaitUntil(OpenSearchConfiguration configuration)
        {
            _tlsEnabled = configuration.TlsEnabled.GetValueOrDefault();
            _username = configuration.Username;
            _password = configuration.Password;
        }

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            using var httpMessageHandler = new HttpClientHandler();
            httpMessageHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            var httpWaitStrategy = new HttpWaitStrategy()
                .UsingHttpMessageHandler(httpMessageHandler)
                .UsingTls(_tlsEnabled)
                .WithBasicAuthentication(_username, _password)
                .ForPort(OpenSearchRestApiPort);

            return await httpWaitStrategy.UntilAsync(container)
                .ConfigureAwait(false);
        }
    }
}