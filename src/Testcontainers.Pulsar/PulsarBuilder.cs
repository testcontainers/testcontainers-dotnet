namespace Testcontainers.Pulsar;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PulsarBuilder : ContainerBuilder<PulsarBuilder, PulsarContainer, PulsarConfiguration>
{
    private const string AuthenticationPlugin = "org.apache.pulsar.client.impl.auth.AuthenticationToken";
    private const string SecretKeyPath = "/pulsar/secret.key";
    private const string UserName = "test-user";
    private const string PulsarImage = "apachepulsar/pulsar:3.0.2";
    private const string AdminClustersEndpoint = "/admin/v2/clusters";
    internal const string Enabled = "Enabled";

    private Dictionary<string, string> _environmentVariables = new Dictionary<string, string>
    {
        { "PULSAR_PREFIX_tokenSecretKey", $"file://{SecretKeyPath}" },
        { "PULSAR_PREFIX_authenticationRefreshCheckSeconds", "5" },
        { "superUserRoles", UserName },
        { "authenticationEnabled", "true" },
        { "authorizationEnabled", "true" },
        { "authenticationProviders", "org.apache.pulsar.broker.authentication.AuthenticationProviderToken" },
        { "authenticateOriginalAuthData", "false" },
        { "brokerClientAuthenticationPlugin", AuthenticationPlugin },
        { "CLIENT_PREFIX_authPlugin", AuthenticationPlugin }
    };

    public const ushort PulsarBrokerPort = 6650;
    public const ushort PulsarBrokerHttpPort = 8080;

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

    /// <inheritdoc />
    public override PulsarContainer Build()
    {
        Validate();
        var pulsarStartupCommands = String.Empty;
        if (DockerResourceConfiguration.Authentication == Enabled)
        {
            pulsarStartupCommands = $"bin/pulsar tokens create-secret-key --output {SecretKeyPath} && " +
                                    $"export brokerClientAuthenticationParameters=token:$(bin/pulsar tokens create --secret-key {SecretKeyPath} --subject {UserName}) && " +
                                    $"export CLIENT_PREFIX_authParams=$brokerClientAuthenticationParameters && bin/apply-config-from-env.py conf/standalone.conf && " +
                                    $"bin/apply-config-from-env-with-prefix.py CLIENT_PREFIX_ conf/client.conf && ";
        }
        pulsarStartupCommands += "bin/pulsar standalone";

        if (DockerResourceConfiguration.Functions != Enabled)
            pulsarStartupCommands += " --no-functions-worker";

        var pulsarBuilder = WithCommand("/bin/bash", "-c",pulsarStartupCommands);
        return new PulsarContainer(pulsarBuilder.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override PulsarBuilder Init()
    {
        return base.Init()
            .WithImage(PulsarImage)
            .WithPortBinding(PulsarBrokerPort, true)
            .WithPortBinding(PulsarBrokerHttpPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilCommandIsCompleted(["/bin/bash", "-c", "bin/pulsar-admin clusters list"]));
    }

    public PulsarBuilder WithTokenAuthentication()
    {
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(authentication: Enabled))
            .WithEnvironment(_environmentVariables);
    }

    public PulsarBuilder WithFunctions()
    {
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(functions: Enabled));
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