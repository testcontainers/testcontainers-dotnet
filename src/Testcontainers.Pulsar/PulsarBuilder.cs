using System.Linq;

namespace Testcontainers.Pulsar;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PulsarBuilder : ContainerBuilder<PulsarBuilder, PulsarContainer, PulsarConfiguration>
{
    public const string PulsarImage = "apachepulsar/pulsar:3.0.4";

    public const ushort PulsarBrokerDataPort = 6650;

    public const ushort PulsarWebServicePort = 8080;

    public const string StartupScriptFilePath = "/testcontainers.sh";

    public const string SecretKeyFilePath = "/pulsar/secret.key";

    public const string Username = "test-user";

    private static readonly IReadOnlyDictionary<string, string> AuthenticationEnvVars;
    
    private static readonly ISet<PulsarService> EnabledServices = new HashSet<PulsarService>();

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
    /// Enables authentication in the Pulsar container.
    /// </summary>
    /// <returns>A <see cref="PulsarBuilder"/> instance.</returns>
    public PulsarBuilder WithAuthentication()
    {
        EnabledServices.Add(PulsarService.Authentication);
        
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(authenticationEnabled: true))
            .WithEnvironment(AuthenticationEnvVars);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WithFunctionsWorkerWorker" /> class.
    /// </summary>
    /// <param name="functionsWorkerEnabled">Determines if the functions worker is enabled.</param>
    /// <returns>A new instance of the <see cref="PulsarBuilder" /> class.</returns>
    public PulsarBuilder WithFunctionsWorker(bool functionsWorkerEnabled = true)
    {
        if (functionsWorkerEnabled)
            EnabledServices.Add(PulsarService.FunctionWorker);
        
        return Merge(DockerResourceConfiguration, new PulsarConfiguration(functionsWorkerEnabled: functionsWorkerEnabled));
    }

    /// <inheritdoc />
    public override PulsarContainer Build()
    {
        Validate();
        
        var waitStrategy = Wait.ForUnixContainer();

        if (EnabledServices.Contains(PulsarService.Authentication))
        {
            waitStrategy.AddCustomWaitStrategy(new WaitUntil());
        }
        else
        {
            waitStrategy = waitStrategy.UntilHttpRequestIsSucceeded(request
                => request
                    .ForPath("/admin/v2/clusters")
                    .ForPort(PulsarWebServicePort)
                    .ForResponseMessageMatching(VerifyPulsarStatusAsync));
        }
        
        if (EnabledServices.Contains(PulsarService.FunctionWorker))
        {
            waitStrategy.UntilMessageIsLogged(".*Function worker service started.*");
        }
        
        var pulsarBuilder =  WithWaitStrategy(waitStrategy);
        return new PulsarContainer(pulsarBuilder.DockerResourceConfiguration);
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
    
    private static async Task<bool> VerifyPulsarStatusAsync(System.Net.Http.HttpResponseMessage response)
    {
        var readAsStringAsync = await response.Content.ReadAsStringAsync();
        return readAsStringAsync == "[\"standalone\"]";
    }
    
    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            try
            {
                var pulsarContainer = container as PulsarContainer;
                var authenticationToken = await pulsarContainer.CreateAuthenticationTokenAsync(TimeSpan.FromSeconds(60),CancellationToken.None);
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri($"http://{container.Hostname}:{container.GetMappedPublicPort(PulsarWebServicePort)}/");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authenticationToken.Replace("\n", ""));
                    System.Net.Http.HttpResponseMessage response = await client.GetAsync("admin/v2/clusters");
                    return await VerifyPulsarStatusAsync(response);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}