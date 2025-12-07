namespace Testcontainers.Pulsar;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PulsarBuilder : ContainerBuilder<PulsarBuilder, PulsarContainer, PulsarConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string PulsarImage = "apachepulsar/pulsar:3.0.9";

    public const ushort PulsarBrokerDataPort = 6650;

    public const ushort PulsarWebServicePort = 8080;

    public const string StartupScriptFilePath = "/testcontainers.sh";

    public const string SecretKeyFilePath = "/tmp/secret.key";

    public const string Username = "test-user";

    private static readonly IReadOnlyDictionary<string, string> AuthenticationEnvVars = InitAuthenticationEnvVars();

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public PulsarBuilder()
        : this(PulsarImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>apachepulsar/pulsar:3.0.9</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/apachepulsar/pulsar/tags" />.
    /// </remarks>
    public PulsarBuilder(string image)
        : this(new PulsarConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/apachepulsar/pulsar/tags" />.
    /// </remarks>
    public PulsarBuilder(IImage image)
        : this(new PulsarConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private PulsarBuilder(PulsarConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override PulsarConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Enables authentication.
    /// </summary>
    /// <remarks>
    /// To create an authentication call <see cref="PulsarContainer.CreateAuthenticationTokenAsync" />.
    /// </remarks>
    /// <returns>A configured instance of <see cref="Pulsar" />.</returns>
    public PulsarBuilder WithAuthentication()
    {
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(authenticationEnabled: true))
            .WithEnvironment(AuthenticationEnvVars);
    }

    /// <summary>
    /// Enables function workers.
    /// </summary>
    /// <param name="functionsWorkerEnabled">Determines whether function workers is enabled or not.</param>
    /// <returns>A configured instance of <see cref="Pulsar" />.</returns>
    public PulsarBuilder WithFunctionsWorker(bool functionsWorkerEnabled = true)
    {
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(functionsWorkerEnabled: functionsWorkerEnabled));
    }

    /// <inheritdoc />
    public override PulsarContainer Build()
    {
        Validate();

        var waitStrategy = Wait.ForUnixContainer();

        if (DockerResourceConfiguration.AuthenticationEnabled.GetValueOrDefault())
        {
            waitStrategy = waitStrategy.UntilFileExists(SecretKeyFilePath, FileSystem.Container);
        }

        if (DockerResourceConfiguration.FunctionsWorkerEnabled.GetValueOrDefault())
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Function worker service started");
        }

        waitStrategy = waitStrategy.AddCustomWaitStrategy(new WaitUntil(DockerResourceConfiguration.AuthenticationEnabled.GetValueOrDefault()));

        var pulsarBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(waitStrategy);
        return new PulsarContainer(pulsarBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override PulsarBuilder Init()
    {
        return base.Init()
            .WithPortBinding(PulsarBrokerDataPort, true)
            .WithPortBinding(PulsarWebServicePort, true)
            .WithFunctionsWorker(false)
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand("while [ ! -f " + StartupScriptFilePath + " ]; do sleep 0.1; done; " + StartupScriptFilePath)
            .WithStartupCallback((container, ct) => container.CopyStartupScriptAsync(ct));
    }

    /// <inheritdoc />
    protected override PulsarBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PulsarBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PulsarBuilder Merge(PulsarConfiguration oldValue, PulsarConfiguration newValue)
    {
        return new PulsarBuilder(new PulsarConfiguration(oldValue, newValue));
    }

    private static IReadOnlyDictionary<string, string> InitAuthenticationEnvVars()
    {
        const string authenticationPlugin = "org.apache.pulsar.client.impl.auth.AuthenticationToken";
        var authenticationEnvVars = new Dictionary<string, string>();
        authenticationEnvVars.Add("authenticateOriginalAuthData", "false");
        authenticationEnvVars.Add("authenticationEnabled", "true");
        authenticationEnvVars.Add("authorizationEnabled", "true");
        authenticationEnvVars.Add("authenticationProviders", "org.apache.pulsar.broker.authentication.AuthenticationProviderToken");
        authenticationEnvVars.Add("brokerClientAuthenticationPlugin", authenticationPlugin);
        authenticationEnvVars.Add("CLIENT_PREFIX_authPlugin", authenticationPlugin);
        authenticationEnvVars.Add("PULSAR_PREFIX_authenticationRefreshCheckSeconds", "5");
        authenticationEnvVars.Add("PULSAR_PREFIX_tokenSecretKey", "file://" + SecretKeyFilePath);
        authenticationEnvVars.Add("superUserRoles", Username);
        return new ReadOnlyDictionary<string, string>(authenticationEnvVars);
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        private readonly HttpWaitStrategy _httpWaitStrategy = new HttpWaitStrategy()
            .ForPath("/admin/v2/namespaces/public/default")
            .ForPort(PulsarWebServicePort);

        private readonly bool _authenticationEnabled;

        private string _authToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntil" /> class.
        /// </summary>
        /// <param name="authenticationEnabled">A value indicating whether authentication is enabled or not.</param>
        public WaitUntil(bool authenticationEnabled)
        {
            _authenticationEnabled = authenticationEnabled;
        }

        /// <inheritdoc />
        public Task<bool> UntilAsync(IContainer container)
        {
            return UntilAsync(container as PulsarContainer);
        }

        /// <inheritdoc cref="IWaitUntil.UntilAsync" />
        private async Task<bool> UntilAsync(PulsarContainer container)
        {
            if (_authenticationEnabled && _authToken == null)
            {
                try
                {
                    _authToken = await container.CreateAuthenticationTokenAsync(TimeSpan.FromHours(1))
                        .ConfigureAwait(false);

                    _ = _httpWaitStrategy.WithHeader("Authorization", "Bearer " + _authToken.Trim());
                }
                catch
                {
                    return false;
                }
            }

            return await _httpWaitStrategy.UntilAsync(container)
                .ConfigureAwait(false);
        }
    }
}