namespace Testcontainers.FlociAz;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class FlociAzContainer : DockerContainer
{
    private const string ResourceNamespaceEnvironmentVariable = "FLOCI_AZ_DOCKER_RESOURCE_NAMESPACE";

    private readonly FlociAzConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociAzContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public FlociAzContainer(FlociAzConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken ct = default)
    {
        await base.StartAsync(ct)
            .ConfigureAwait(false);

        if (Guid.Empty.Equals(_configuration.SessionId)
            || !_configuration.Environments.TryGetValue(ResourceNamespaceEnvironmentVariable, out var resourceNamespace)
            || string.IsNullOrEmpty(resourceNamespace))
        {
            return;
        }

        var resourceReaper = await ResourceReaper.GetAndStartDefaultAsync(_configuration.DockerEndpointAuthConfig, _configuration.Logger, ct: ct)
            .ConfigureAwait(false);
        await resourceReaper.RegisterFilterAsync($"label=floci_namespace={resourceNamespace}", ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the FlociAz storage connection string.
    /// </summary>
    /// <returns>The FlociAz storage connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("DefaultEndpointsProtocol", Uri.UriSchemeHttp);
        properties.Add("AccountName", FlociAzBuilder.AccountName);
        properties.Add("AccountKey", FlociAzBuilder.AccountKey);
        properties.Add("BlobEndpoint", GetServiceEndpoint());
        properties.Add("QueueEndpoint", GetServiceEndpoint("queue"));
        properties.Add("TableEndpoint", GetServiceEndpoint("table"));
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Gets the FlociAz endpoint used by root-level and Azure Resource Manager APIs.
    /// </summary>
    /// <returns>The FlociAz endpoint.</returns>
    public string GetEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(FlociAzBuilder.FlociAzPort)).ToString();
    }

    /// <summary>
    /// Gets the FlociAz service endpoint.
    /// </summary>
    /// <remarks>
    /// FlociAz routes its REST services by path: the blob storage service uses the
    /// bare <c>/{accountName}</c> path (omit the <paramref name="service" /> argument),
    /// other services use <c>/{accountName}-{service}</c> (e.g., <c>queue</c>,
    /// <c>table</c>, <c>cosmos</c>, <c>keyvault</c>, <c>appconfig</c>, <c>functions</c>).
    /// </remarks>
    /// <param name="service">The service name, or <c>null</c> for the blob storage endpoint.</param>
    /// <returns>The FlociAz service endpoint.</returns>
    public string GetServiceEndpoint(string service = null)
    {
        var path = string.IsNullOrEmpty(service) ? FlociAzBuilder.AccountName : string.Join("-", FlociAzBuilder.AccountName, service);
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(FlociAzBuilder.FlociAzPort), path + "/").ToString();
    }

    /// <summary>
    /// Gets the host port mapped to a FlociAz sidecar container port.
    /// </summary>
    /// <param name="sidecarHostname">The sidecar hostname returned by FlociAz.</param>
    /// <param name="privatePort">The sidecar container port.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The Docker host port mapped to <paramref name="privatePort" />.</returns>
    /// <exception cref="InvalidOperationException">
    /// The container was not configured with <see cref="FlociAzBuilder.WithDockerSocket" />, or
    /// the requested sidecar or port does not exist.
    /// </exception>
    public async Task<ushort> GetSidecarMappedPublicPortAsync(string sidecarHostname, ushort privatePort, CancellationToken ct = default)
    {
        if (!_configuration.Environments.TryGetValue(ResourceNamespaceEnvironmentVariable, out var resourceNamespace)
            || string.IsNullOrEmpty(resourceNamespace))
        {
            throw new InvalidOperationException($"Configure the container with {nameof(FlociAzBuilder)}.{nameof(FlociAzBuilder.WithDockerSocket)}() before resolving sidecar ports.");
        }

        using var dockerClient = _configuration.DockerEndpointAuthConfig.GetDockerClientBuilder(_configuration.SessionId).Build();
        var filters = new Dictionary<string, IDictionary<string, bool>>
        {
            ["label"] = new Dictionary<string, bool> { [$"floci_namespace={resourceNamespace}"] = true },
        };
        var sidecars = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true, Filters = filters }, ct)
            .ConfigureAwait(false);
        var sidecar = sidecars.SingleOrDefault(container => container.Names.Any(name => sidecarHostname.Equals(name.TrimStart('/'), StringComparison.Ordinal)));
        var port = sidecar?.Ports.FirstOrDefault(binding => binding.PrivatePort == privatePort && binding.PublicPort > 0);

        return port == null
            ? throw new InvalidOperationException($"FlociAz sidecar '{sidecarHostname}' does not expose container port {privatePort}.")
            : checked((ushort)port.PublicPort);
    }

    /// <summary>
    /// Gets the FlociAz Cosmos DB connection string.
    /// </summary>
    /// <returns>The FlociAz Cosmos DB connection string.</returns>
    public string GetCosmosConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("AccountEndpoint", GetServiceEndpoint("cosmos"));
        properties.Add("AccountKey", FlociAzBuilder.AccountKey);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }
}
