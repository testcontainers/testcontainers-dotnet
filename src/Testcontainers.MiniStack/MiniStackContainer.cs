namespace Testcontainers.MiniStack;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MiniStackContainer : DockerContainer
{
    private const string InstanceLabel = "ministack.instance";

    private static readonly TimeSpan CleanupTimeout = TimeSpan.FromMinutes(1);

    private static readonly HttpClient HttpClient = new HttpClient();

    private readonly MiniStackConfiguration _configuration;

    private string _instanceId;

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MiniStackContainer(MiniStackConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the MiniStack connection string.
    /// </summary>
    /// <remarks>
    /// Use it as the AWS SDK <c>ServiceURL</c>.
    /// </remarks>
    /// <returns>The MiniStack connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(MiniStackBuilder.MiniStackPort)).ToString();
    }

    /// <summary>
    /// Gets the AWS region: the value passed to <see cref="MiniStackBuilder.WithRegion" />,
    /// or <c>us-east-1</c>.
    /// </summary>
    /// <returns>The AWS region.</returns>
    public string GetRegion()
    {
        return GetEnvironmentOrDefault(MiniStackBuilder.RegionEnvVar, MiniStackBuilder.Region);
    }

    /// <summary>
    /// Gets the AWS access key ID: the value passed to <see cref="MiniStackBuilder.WithCredentials" />,
    /// or <c>test</c>.
    /// </summary>
    /// <returns>The AWS access key ID.</returns>
    public string GetAccessKey()
    {
        return GetEnvironmentOrDefault(MiniStackBuilder.AccessKeyEnvVar, MiniStackBuilder.AccessKey);
    }

    /// <summary>
    /// Gets the AWS secret access key: the value passed to <see cref="MiniStackBuilder.WithCredentials" />,
    /// or <c>test</c>.
    /// </summary>
    /// <returns>The AWS secret access key.</returns>
    public string GetSecretKey()
    {
        return GetEnvironmentOrDefault(MiniStackBuilder.SecretKeyEnvVar, MiniStackBuilder.SecretKey);
    }

    /// <summary>
    /// Gets the MiniStack version, which is the Docker image tag the container was built with.
    /// </summary>
    /// <remarks>
    /// Returns the tag verbatim. Semver tags such as <c>1.3.42</c> can be parsed with
    /// <c>Version.TryParse</c> for capability gating;
    /// <c>latest</c> and <c>nightly</c> cannot, so pin a version tag when that matters.
    /// An image referenced only by digest has no tag; use <see cref="GetMiniStackVersionAsync" /> instead.
    /// </remarks>
    /// <returns>The image tag, e.g. <c>1.3.42</c> or <c>latest</c>, or <c>null</c> if the image has no tag.</returns>
    public string GetMiniStackVersion()
    {
        return Image.Tag;
    }

    /// <summary>
    /// Gets the version the running MiniStack server reports on its health endpoint.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="GetMiniStackVersion" />, this resolves floating tags such as
    /// <c>latest</c> to the actual release. The container must be running.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The server version, e.g. <c>1.5.31</c>, or <c>null</c> if the health response has none.</returns>
    public async Task<string> GetMiniStackVersionAsync(CancellationToken ct = default)
    {
        using var response = await HttpClient.GetAsync(new Uri(new Uri(GetConnectionString()), MiniStackBuilder.HealthPath), ct)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        using var health = JsonDocument.Parse(json);
        return health.RootElement.TryGetProperty("version", out var version) && version.ValueKind == JsonValueKind.String ? version.GetString() : null;
    }

    /// <inheritdoc />
    protected override async Task UnsafeStartContainerAsync(CancellationToken ct = default)
    {
        await base.UnsafeStartContainerAsync(ct)
            .ConfigureAwait(false);

        // Resolve the label before the readiness checks: ready scripts can create nested
        // containers, and a start that fails or times out must still clean them up.
        _instanceId = await ResolveInstanceIdAsync(ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task UnsafeStartAsync(CancellationToken ct = default)
    {
        await base.UnsafeStartAsync(ct)
            .ConfigureAwait(false);

        await ThrowIfReadyScriptsFailedAsync(ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Also removes the nested containers and volumes this MiniStack started. Removing the
    /// container kills MiniStack, so its own shutdown cleanup does not run. A plain stop
    /// lets MiniStack shut down and remove them itself.
    /// </remarks>
    protected override async Task UnsafeDeleteAsync(CancellationToken ct = default)
    {
        try
        {
            await base.UnsafeDeleteAsync(ct)
                .ConfigureAwait(false);
        }
        finally
        {
            await ReapMiniStackResourcesAsync(ct)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Throws if MiniStack reports a failed ready script.
    /// </summary>
    /// <remarks>
    /// MiniStack marks the ready scripts as completed even when one of them fails, and
    /// only counts the failure. Images without the readiness endpoint are not checked.
    /// </remarks>
    private async Task ThrowIfReadyScriptsFailedAsync(CancellationToken ct)
    {
        // The readiness wait has just succeeded against the same endpoint, so a failing
        // request here is a real error and propagates instead of skipping the check.
        using var response = await HttpClient.GetAsync(new Uri(new Uri(GetConnectionString()), MiniStackBuilder.ReadyPath), ct)
            .ConfigureAwait(false);

        if (HttpStatusCode.NotFound.Equals(response.StatusCode))
        {
            return;
        }

        if (!HttpStatusCode.OK.Equals(response.StatusCode))
        {
            // Only possible with a custom wait strategy that does not wait for the ready scripts.
            Logger.LogWarning("MiniStack ready scripts are still running (HTTP {StatusCode}); their failures are not checked", (int)response.StatusCode);
            return;
        }

        var json = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        using var ready = JsonDocument.Parse(json);

        if (ready.RootElement.TryGetProperty("failed", out var failed) && failed.ValueKind == JsonValueKind.Number && failed.GetInt32() > 0)
        {
            throw new InvalidOperationException($"{failed.GetInt32()} MiniStack ready script(s) failed. The container logs contain the script output.");
        }
    }

    /// <summary>
    /// Gets a configured environment variable, or the default if it is unset or empty.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <param name="defaultValue">The value MiniStack uses when the variable is not set.</param>
    /// <returns>The configured value or the default.</returns>
    private string GetEnvironmentOrDefault(string name, string defaultValue)
    {
        return _configuration.Environments.TryGetValue(name, out var value) && !string.IsNullOrEmpty(value) ? value : defaultValue;
    }

    /// <summary>
    /// Creates a Docker client for the endpoint and session the container uses.
    /// </summary>
    /// <returns>A Docker client that the caller disposes.</returns>
    private Docker.DotNet.IDockerClient CreateDockerClient()
    {
        return _configuration.DockerEndpointAuthConfig.GetDockerClientBuilder(_configuration.SessionId).Build();
    }

    /// <summary>
    /// Works out the <c>ministack.instance</c> label value MiniStack stamps on every
    /// container it spawns: <c>{MINISTACK_HOSTNAME or container hostname}:{GATEWAY_PORT or EDGE_PORT or 4566}</c>.
    /// </summary>
    private async Task<string> ResolveInstanceIdAsync(CancellationToken ct)
    {
        try
        {
            string host;
            if (!_configuration.Environments.TryGetValue("MINISTACK_HOSTNAME", out host) || string.IsNullOrEmpty(host))
            {
                using var client = CreateDockerClient();
                var inspect = await client.Containers.InspectContainerAsync(Id, ct)
                    .ConfigureAwait(false);
                host = inspect.Config.Hostname;
            }

            var port = GetEnvironmentOrDefault("GATEWAY_PORT", GetEnvironmentOrDefault("EDGE_PORT", MiniStackBuilder.MiniStackPort.ToString(CultureInfo.InvariantCulture)));
            return host + ":" + port;
        }
        catch (Exception e) when (!(e is OperationCanceledException))
        {
            Logger.LogWarning(e, "Could not resolve the MiniStack instance id; nested containers will not be removed");
            return null;
        }
    }

    /// <summary>
    /// Removes the nested containers and volumes this MiniStack spawned for
    /// real-infrastructure services (RDS, ElastiCache, ECS, EKS, Lambda).
    /// </summary>
    /// <remarks>
    /// They run on the host engine, not inside the MiniStack container, so the
    /// Resource Reaper does not see them. Only resources labelled with this
    /// container's <c>ministack.instance</c> are removed. MiniStack derives that value
    /// from <c>MINISTACK_HOSTNAME</c> (or the container hostname) and the gateway port,
    /// so instances that share both values also share their nested containers. Cleanup
    /// is best-effort and bounded by a timeout: failures are logged and never thrown, so
    /// a stuck remove never masks a test failure.
    /// </remarks>
    private async Task ReapMiniStackResourcesAsync(CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_instanceId))
        {
            return;
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(CleanupTimeout);

        var labelFilter = new Dictionary<string, IDictionary<string, bool>>
        {
            ["label"] = new Dictionary<string, bool> { [InstanceLabel + "=" + _instanceId] = true },
        };

        Docker.DotNet.IDockerClient client;
        try
        {
            client = CreateDockerClient();
        }
        catch (Exception e)
        {
            Logger.LogWarning(e, "MiniStack cleanup skipped, Docker client unavailable");
            return;
        }

        using (client)
        {
            try
            {
                var orphans = await client.Containers.ListContainersAsync(new ContainersListParameters { All = true, Filters = labelFilter }, cts.Token)
                    .ConfigureAwait(false);

                foreach (var orphan in orphans)
                {
                    try
                    {
                        await client.Containers.RemoveContainerAsync(orphan.ID, new ContainerRemoveParameters { Force = true, RemoveVolumes = true }, cts.Token)
                            .ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning(e, "Failed to remove MiniStack container {Id}", orphan.ID);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, "Failed to list MiniStack containers for cleanup");
            }

            try
            {
                var volumes = await client.Volumes.ListAsync(new VolumesListParameters { Filters = labelFilter }, cts.Token)
                    .ConfigureAwait(false);

                foreach (var volume in volumes.Volumes ?? new List<VolumeResponse>())
                {
                    try
                    {
                        await client.Volumes.RemoveAsync(volume.Name, null, cts.Token)
                            .ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Logger.LogDebug(e, "Failed to remove MiniStack volume {Name}", volume.Name);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, "Failed to list MiniStack volumes for cleanup");
            }
        }
    }
}