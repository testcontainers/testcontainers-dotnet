namespace Testcontainers.Pulsar;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PulsarBuilder : ContainerBuilder<PulsarBuilder, PulsarContainer, PulsarConfiguration>
{
    public const string PulsarImage = "apachepulsar/pulsar:3.0.3";

    public const ushort PulsarBrokerDataPort = 6650;

    public const ushort PulsarWebServicePort = 8080;

    public const string StartupScriptFilePath = "/testcontainers.sh";

    public const string SecretKeyFilePath = "/pulsar/secret.key";

    public const string Username = "test-user";

    private static readonly IReadOnlyDictionary<string, string> AuthenticationEnvVars;

    static PulsarBuilder()
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
        AuthenticationEnvVars = new ReadOnlyDictionary<string, string>(authenticationEnvVars);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarBuilder" /> class.
    /// </summary>
    public PulsarBuilder()
        : this(new PulsarConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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
    ///
    /// </summary>
    /// <returns></returns>
    public PulsarBuilder WithAuthentication()
    {
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(authenticationEnabled: true))
            .WithEnvironment(AuthenticationEnvVars);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public PulsarBuilder WithFunctionsWorker(bool functionsWorkerEnabled = true)
    {
        // TODO: When enabled we need to adjust the wait strategy.
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(functionsWorkerEnabled: functionsWorkerEnabled));
    }

    /// <inheritdoc />
    public override PulsarContainer Build()
    {
        Validate();
        return new PulsarContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override PulsarBuilder Init()
    {
        return base.Init()
            .WithImage(PulsarImage)
            .WithPortBinding(PulsarBrokerDataPort, true)
            .WithPortBinding(PulsarWebServicePort, true)
            .WithFunctionsWorker(false)
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand("while [ ! -f " + StartupScriptFilePath + " ]; do sleep 0.1; done; " + StartupScriptFilePath)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("bin/pulsar-admin", "clusters", "list"))
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
}