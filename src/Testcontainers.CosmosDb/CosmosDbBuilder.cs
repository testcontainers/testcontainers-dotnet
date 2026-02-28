namespace Testcontainers.CosmosDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class CosmosDbBuilder : ContainerBuilder<CosmosDbBuilder, CosmosDbContainer, CosmosDbConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string CosmosDbImage = "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:vnext-preview";

    public const ushort CosmosDbPort = 8081;
    public const ushort CosmosDbHealthCheckPort = 8080;
    public const string DefaultAccountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public CosmosDbBuilder()
        : this(CosmosDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/microsoft/azure-cosmosdb-emulator" />.
    /// </remarks>
    public CosmosDbBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/microsoft/azure-cosmosdb-emulator" />.
    /// </remarks>
    public CosmosDbBuilder(IImage image)
        : this(new CosmosDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private CosmosDbBuilder(CosmosDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override CosmosDbConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override CosmosDbContainer Build()
    {
        Validate();
        return new CosmosDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Init()
    {
        return base.Init()
            .WithPortBinding(CosmosDbPort, true)
            .WithPortBinding(CosmosDbHealthCheckPort, true)
            .WithEnvironment("ENABLE_EXPLORER", "false")
            .WithConnectionStringProvider(new CosmosDbConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()));
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CosmosDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CosmosDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Merge(CosmosDbConfiguration oldValue, CosmosDbConfiguration newValue)
    {
        return new CosmosDbBuilder(new CosmosDbConfiguration(oldValue, newValue));
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            // CosmosDB's preconfigured HTTP client will redirect the request to the container.
            const string REQUEST_URI = "http://localhost/alive";

            using var httpClient = ((CosmosDbContainer)container).HttpClientHealthCheck;

            try
            {
                using var httpResponse = await httpClient
                    .GetAsync(REQUEST_URI)
                    .ConfigureAwait(false);

                /*
                    Example response from CosmosDB Emulator's /alive endpoint:

                    HTTP/1.1 200 OK
                    Content-Type: application/json
                    Connection: close
                    Content-Length: 280

                    {
                        "alive": true,
                        "checks": {
                            "explorer": "disabled",
                            "gateway": "healthy",
                            "postgres": "healthy"
                        },
                        "overall": true,
                        "protocol": "http",
                        "ready": true,
                        "status": "healthy",
                        "timestamp": "2026-02-28T08:52:40.328000942+00:00",
                        "version": "EN20260227"
                    }

                    The following conditions will check that the endpoint returns a success status code
                        and that the "gateway" and "postgres" checks are healthy, which indicates that the CosmosDB Emulator is ready to accept requests.
                    This is because sometimes the /alive endpoint may return a successful response before the CosmosDB Emulator is fully ready
                        to accept requests. Checking the "gateway" and "postgres" checks can provide a more reliable indication of readiness.
                */
                if (httpResponse.IsSuccessStatusCode)
                {
                    var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    using var jsonDocument = System.Text.Json.JsonDocument.Parse(content);
                    if (jsonDocument.RootElement.TryGetProperty("checks", out var checksProperty) &&
                        checksProperty.TryGetProperty("gateway", out var gatewayProperty) &&
                        "healthy".Equals(gatewayProperty.GetString(), StringComparison.OrdinalIgnoreCase) &&
                        checksProperty.TryGetProperty("postgres", out var postgresProperty) &&
                        "healthy".Equals(postgresProperty.GetString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }
    }
}
