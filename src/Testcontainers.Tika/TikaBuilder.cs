namespace Testcontainers.Tika;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class TikaBuilder : ContainerBuilder<TikaBuilder, TikaContainer, TikaConfiguration>
{
    public const string TikaImage = "apache/tika:3.0.0.0-full";
    public const ushort TikaHttpPort = 9998;

    /// <summary>
    /// Initializes a new instance of the <see cref="TikaBuilder" /> class.
    /// </summary>
    public TikaBuilder()
        : this(new TikaConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TikaBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private TikaBuilder(TikaConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }
    /// <inheritdoc />

    protected override TikaConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Tika server timeout.
    /// </summary>
    /// <param name="timeout">The timeout for the server in milliseconds.</param>
    /// <returns>A configured instance of <see cref="TikaBuilder" />.</returns>
    public TikaBuilder WithTimeout(int timeout)
    {
        return Merge(DockerResourceConfiguration, new TikaConfiguration(timeout: timeout))
            .WithEnvironment("TIKA_TIMEOUT", timeout.ToString());
    }

    public override TikaContainer Build()
    {
        Validate();
        return new TikaContainer(DockerResourceConfiguration);
    }

    protected override TikaBuilder Init()
    {
        return base.Init()
            .WithImage(TikaImage)
            .WithPortBinding(TikaHttpPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()));
    }

    protected override TikaBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new TikaConfiguration(resourceConfiguration));
    }

    protected override TikaBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new TikaConfiguration(resourceConfiguration));
    }

    protected override TikaBuilder Merge(TikaConfiguration oldValue, TikaConfiguration newValue)
    {
        return new TikaBuilder(new TikaConfiguration(oldValue, newValue));
    }

    private sealed class WaitUntil : IWaitUntil
    {
        private const string HealthCheckPath = "tika";
        private const int MaxRetryAttempts = 10;
        private const int DelayInMilliseconds = 1000;

        /// <summary>
        /// Waits until the Tika server is available by checking the health check endpoint.
        /// </summary>
        /// <param name="container">The container instance to check.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean indicating whether the Tika server is available.
        /// </returns>
        /// <remarks>
        /// This method sends HTTP GET requests to the Tika server's health check endpoint and retries up to a maximum number of times if the server is not available.
        /// </remarks>
        public async Task<bool> UntilAsync(DotNet.Testcontainers.Containers.IContainer container)
        {
            string endpoint = $"http://{container.Hostname}:{container.GetMappedPublicPort(TikaBuilder.TikaHttpPort)}/{HealthCheckPath}";

            using var client = new HttpClient();

            for (int i = 0; i < MaxRetryAttempts; i++)
            {
                try
                {
                    var response = await client.GetAsync(endpoint);

                    response.EnsureSuccessStatusCode();
                    string responseContent = await response.Content.ReadAsStringAsync(); // This is Tika Server (Apache Tika 3.0.0). Please PUT, volendo si può fare questo check
                    return true;
                }
                catch
                {
                    // Ignore exceptions and retry
                }

                await Task.Delay(DelayInMilliseconds);
            }

            return false;
        }
    }
}
