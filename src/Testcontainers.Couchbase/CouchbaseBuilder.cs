namespace Testcontainers.Couchbase;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
/// <remarks>
/// The Couchbase module runs the following services: Data, Index, Query, Search and creates the <see cref="CouchbaseBucket.Default" /> bucket during the start.
/// </remarks>
[PublicAPI]
public sealed class CouchbaseBuilder : ContainerBuilder<CouchbaseBuilder, CouchbaseContainer, CouchbaseConfiguration>
{
    public const string CouchbaseImage = "couchbase:community-7.0.2";

    public const ushort MgmtPort = 8091;

    public const ushort MgmtSslPort = 18091;

    public const ushort ViewPort = 8092;

    public const ushort ViewSslPort = 18092;

    public const ushort QueryPort = 8093;

    public const ushort QuerySslPort = 18093;

    public const ushort SearchPort = 8094;

    public const ushort SearchSslPort = 18094;

    public const ushort AnalyticsPort = 8095;

    public const ushort AnalyticsSslPort = 18095;

    public const ushort EventingPort = 8096;

    public const ushort EventingSslPort = 18096;

    public const ushort KvPort = 11210;

    public const ushort KvSslPort = 11207;

    public const string DefaultUsername = "Administrator";

    public const string DefaultPassword = "password";

    private static readonly KeyValuePair<string, string> BasicAuthenticationHeader = new KeyValuePair<string, string>("Authorization", "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(string.Join(":", DefaultUsername, DefaultPassword))));

    private static readonly IWaitUntil WaitUntilNodeIsReady = new HttpWaitStrategy().ForPath("/pools").ForPort(MgmtPort);

    private static readonly ISet<CouchbaseService> EnabledServices = new HashSet<CouchbaseService>();

    static CouchbaseBuilder()
    {
        EnabledServices.Add(CouchbaseService.Data);
        EnabledServices.Add(CouchbaseService.Index);
        EnabledServices.Add(CouchbaseService.Query);
        EnabledServices.Add(CouchbaseService.Search);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseBuilder" /> class.
    /// </summary>
    public CouchbaseBuilder()
        : this(new CouchbaseConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private CouchbaseBuilder(CouchbaseConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override CouchbaseConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override CouchbaseContainer Build()
    {
        Validate();

        var waitStrategy = Wait.ForUnixContainer();

        if (EnabledServices.Any())
        {
            waitStrategy = waitStrategy.UntilHttpRequestIsSucceeded(request
                => request
                    .ForPath("/pools/default")
                    .ForPort(MgmtPort)
                    .ForResponseMessageMatching(IsNodeHealthyAsync)
                    .WithHeader(BasicAuthenticationHeader.Key, BasicAuthenticationHeader.Value));
        }

        if (EnabledServices.Contains(CouchbaseService.Query))
        {
            waitStrategy = waitStrategy.UntilHttpRequestIsSucceeded(request
                => request
                    .ForPath("/admin/ping")
                    .ForPort(QueryPort)
                    .WithHeader(BasicAuthenticationHeader.Key, BasicAuthenticationHeader.Value));
        }

        if (EnabledServices.Contains(CouchbaseService.Analytics))
        {
            waitStrategy = waitStrategy.UntilHttpRequestIsSucceeded(request
                => request
                    .ForPath("/admin/ping")
                    .ForPort(AnalyticsPort)
                    .WithHeader(BasicAuthenticationHeader.Key, BasicAuthenticationHeader.Value));
        }

        if (EnabledServices.Contains(CouchbaseService.Eventing))
        {
            waitStrategy = waitStrategy.UntilHttpRequestIsSucceeded(request
                => request
                    .ForPath("/api/v1/config")
                    .ForPort(EventingPort)
                    .WithHeader(BasicAuthenticationHeader.Key, BasicAuthenticationHeader.Value));
        }

        var couchbaseBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(waitStrategy);
        return new CouchbaseContainer(couchbaseBuilder.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override CouchbaseBuilder Init()
    {
        return base.Init()
            .WithImage(CouchbaseImage)
            .WithPortBinding(MgmtPort, true)
            .WithPortBinding(MgmtSslPort, true)
            .WithPortBinding(ViewPort, true)
            .WithPortBinding(ViewSslPort, true)
            .WithPortBinding(QueryPort, true)
            .WithPortBinding(QuerySslPort, true)
            .WithPortBinding(SearchPort, true)
            .WithPortBinding(SearchSslPort, true)
            .WithPortBinding(AnalyticsPort, true)
            .WithPortBinding(AnalyticsSslPort, true)
            .WithPortBinding(EventingPort, true)
            .WithPortBinding(EventingSslPort, true)
            .WithPortBinding(KvPort, true)
            .WithPortBinding(KvSslPort, true)
            .WithBucket(CouchbaseBucket.Default)
            .WithStartupCallback(ConfigureCouchbaseAsync);
    }

    /// <inheritdoc />
    protected override CouchbaseBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CouchbaseConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CouchbaseBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CouchbaseConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CouchbaseBuilder Merge(CouchbaseConfiguration oldValue, CouchbaseConfiguration newValue)
    {
        return new CouchbaseBuilder(new CouchbaseConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Adds a Couchbase bucket.
    /// </summary>
    /// <param name="bucket">The Couchbase bucket.</param>
    /// <returns>A configured instance of <see cref="CouchbaseBuilder" />.</returns>
    private CouchbaseBuilder WithBucket(params CouchbaseBucket[] bucket)
    {
        return Merge(DockerResourceConfiguration, new CouchbaseConfiguration(buckets: bucket));
    }

    /// <summary>
    /// Configures the Couchbase node.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="ct">Cancellation token.</param>
    private async Task ConfigureCouchbaseAsync(IContainer container, CancellationToken ct = default)
    {
        await WaitStrategy.WaitUntilAsync(() => WaitUntilNodeIsReady.UntilAsync(container), TimeSpan.FromSeconds(2), TimeSpan.FromMinutes(5), ct)
            .ConfigureAwait(false);

        using (var httpClient = new HttpClient(new RetryHandler()))
        {
            httpClient.BaseAddress = new UriBuilder(Uri.UriSchemeHttp, container.Hostname, container.GetMappedPublicPort(MgmtPort)).Uri;

            using (var request = new RenameNodeRequest(container))
            {
                using (var response = await httpClient.SendAsync(request, ct)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccessStatusCodeAsync(response)
                        .ConfigureAwait(false);
                }
            }

            using (var request = new SetupNodeServicesRequest(EnabledServices.ToArray()))
            {
                using (var response = await httpClient.SendAsync(request, ct)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccessStatusCodeAsync(response)
                        .ConfigureAwait(false);
                }
            }

            using (var request = new SetupMemoryQuotasRequest(EnabledServices.ToArray()))
            {
                using (var response = await httpClient.SendAsync(request, ct)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccessStatusCodeAsync(response)
                        .ConfigureAwait(false);
                }
            }

            using (var request = new ConfigureExternalAddressesRequest(container, EnabledServices.ToArray()))
            {
                using (var response = await httpClient.SendAsync(request, ct)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccessStatusCodeAsync(response)
                        .ConfigureAwait(false);
                }
            }

            foreach (var bucket in DockerResourceConfiguration.Buckets)
            {
                using (var request = new CreateBucketRequest(bucket))
                {
                    using (var response = await httpClient.SendAsync(request, ct)
                        .ConfigureAwait(false))
                    {
                        await EnsureSuccessStatusCodeAsync(response)
                            .ConfigureAwait(false);
                    }
                }
            }

            // This HTTP request initiates the provisioning of the single-node cluster.
            // All subsequent requests following this HTTP request require credentials.
            // Setting the credentials upfront interfere with other HTTP requests.
            // We got frequently: System.IO.IOException The response ended prematurely.
            using (var request = new SetupCredentialsRequest())
            {
                using (var response = await httpClient.SendAsync(request, ct)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccessStatusCodeAsync(response)
                        .ConfigureAwait(false);
                }
            }
        }

        // As long as we do not expose the bucket API, we do not need to iterate over all of them.
        var waitUntilBucketIsCreated = DockerResourceConfiguration.Buckets.Aggregate(Wait.ForUnixContainer(), (waitStrategy, bucket)
            => waitStrategy.UntilHttpRequestIsSucceeded(request
                => request
                    .ForPath("/pools/default/buckets/" + bucket.Name)
                    .ForPort(MgmtPort)
                    .ForResponseMessageMatching(AllServicesEnabledAsync)
                    .WithHeader(BasicAuthenticationHeader.Key, BasicAuthenticationHeader.Value)))
            .Build()
            .Last();

        await WaitStrategy.WaitUntilAsync(() => waitUntilBucketIsCreated.UntilAsync(container), TimeSpan.FromSeconds(2), TimeSpan.FromMinutes(5), ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Determines whether the single-node is healthy or not.
    /// </summary>
    /// <remarks>
    /// https://docs.couchbase.com/server/current/rest-api/rest-cluster-get.html#http-method-and-uri
    /// </remarks>
    /// <param name="response">The HTTP response that contains the cluster information.</param>
    /// <returns>A value indicating whether the single-node is healthy or not.</returns>
    private async Task<bool> IsNodeHealthyAsync(HttpResponseMessage response)
    {
        var jsonString = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        try
        {
            var status = JsonDocument.Parse(jsonString)
                .RootElement
                .GetProperty("nodes")
                .EnumerateArray()
                .ElementAt(0)
                .GetProperty("status")
                .GetString();

            return "healthy".Equals(status);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether all services are enabled for a bucket or not.
    /// </summary>
    /// <remarks>
    /// https://docs.couchbase.com/server/current/rest-api/rest-buckets-summary.html#http-methods-and-uris.
    /// </remarks>
    /// <param name="response">The HTTP response that contains the bucket information.</param>
    /// <returns>A value indicating whether all services are enabled for a bucket or not.</returns>
    private async Task<bool> AllServicesEnabledAsync(HttpResponseMessage response)
    {
        var jsonString = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        try
        {
            var services = JsonDocument.Parse(jsonString)
                .RootElement
                .GetProperty("nodes")
                .EnumerateArray()
                .ElementAt(0)
                .GetProperty("services")
                .EnumerateArray()
                .Select(service => service.GetString())
                .Where(service => service != null);

            return EnabledServices.All(enabledService => services.Any(service => service.StartsWith(enabledService.Identifier)));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Throws an exception if the <see cref="HttpResponseMessage.IsSuccessStatusCode" /> property for the HTTP response is <see langword="false" />
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <exception cref="InvalidOperationException">The HTTP response is unsuccessful.</exception>
    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            var content = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            throw new InvalidOperationException(content, e);
        }
    }

    /// <summary>
    /// An HTTP request that renames the Couchbase node.
    /// </summary>
    private sealed class RenameNodeRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenameNodeRequest" /> class.
        /// </summary>
        /// <remarks>
        /// https://docs.couchbase.com/server/current/rest-api/rest-name-node.html#http-method-and-uri.
        /// </remarks>
        /// <param name="container">The Couchbase container.</param>
        public RenameNodeRequest(IContainer container)
            : base(HttpMethod.Post, "/node/controller/rename")
        {
            IDictionary<string, string> content = new Dictionary<string, string>();
            content.Add("hostname", container.IpAddress);
            Content = new FormUrlEncodedContent(content);
        }
    }

    /// <summary>
    /// An HTTP request that enables the Couchbase node services.
    /// </summary>
    private sealed class SetupNodeServicesRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetupNodeServicesRequest" /> class.
        /// </summary>
        /// <remarks>
        /// https://docs.couchbase.com/server/current/rest-api/rest-set-up-services.html#http-method-and-uri.
        /// </remarks>
        /// <param name="enabledServices">The enabled Couchbase node services.</param>
        public SetupNodeServicesRequest(params CouchbaseService[] enabledServices)
            : base(HttpMethod.Post, "/node/controller/setupServices")
        {
            IDictionary<string, string> content = new Dictionary<string, string>();
            content.Add("services", string.Join(",", enabledServices.Select(enabledService => enabledService.Identifier)));
            Content = new FormUrlEncodedContent(content);
        }
    }

    /// <summary>
    /// An HTTP request that sets the Couchbase node service memory quotas.
    /// </summary>
    private sealed class SetupMemoryQuotasRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetupNodeServicesRequest" /> class.
        /// </summary>
        /// <remarks>
        /// https://docs.couchbase.com/server/current/rest-api/rest-configure-memory.html#http-method-and-uri
        /// </remarks>
        /// <param name="enabledServices">The enabled Couchbase node services.</param>
        public SetupMemoryQuotasRequest(params CouchbaseService[] enabledServices)
            : base(HttpMethod.Post, "/pools/default")
        {
            IDictionary<string, string> content = new Dictionary<string, string>();

            foreach (var enabledService in enabledServices)
            {
                if (!enabledService.HasQuota)
                {
                    continue;
                }

                if (CouchbaseService.Data.Equals(enabledService))
                {
                    content.Add("memoryQuota", enabledService.MinimumQuotaMb.ToString());
                }
                else
                {
                    content.Add(enabledService.Identifier + "MemoryQuota", enabledService.MinimumQuotaMb.ToString());
                }
            }

            Content = new FormUrlEncodedContent(content);
        }
    }

    /// <summary>
    /// An HTTP request that configures the Couchbase node external addresses.
    /// </summary>
    private sealed class ConfigureExternalAddressesRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureExternalAddressesRequest" /> class.
        /// </summary>
        /// <remarks>
        /// https://docs.couchbase.com/server/current/rest-api/rest-set-up-alternate-address.html#http-method-and-uri.
        /// </remarks>
        /// <param name="container">The Couchbase container.</param>
        /// <param name="enabledServices">The enabled Couchbase node services.</param>
        public ConfigureExternalAddressesRequest(IContainer container, params CouchbaseService[] enabledServices)
            : base(HttpMethod.Put, "/node/controller/setupAlternateAddresses/external")
        {
            IDictionary<string, string> content = new Dictionary<string, string>();

            content.Add("hostname", container.Hostname);
            content.Add("mgmt", container.GetMappedPublicPort(MgmtPort).ToString());
            content.Add("mgmtSSL", container.GetMappedPublicPort(MgmtSslPort).ToString());

            if (enabledServices.Contains(CouchbaseService.Data))
            {
                content.Add("kv", container.GetMappedPublicPort(KvPort).ToString());
                content.Add("kvSSL", container.GetMappedPublicPort(KvSslPort).ToString());
                content.Add("capi", container.GetMappedPublicPort(ViewPort).ToString());
                content.Add("capiSSL", container.GetMappedPublicPort(ViewSslPort).ToString());
            }

            if (enabledServices.Contains(CouchbaseService.Query))
            {
                content.Add("n1ql", container.GetMappedPublicPort(QueryPort).ToString());
                content.Add("n1qlSSL", container.GetMappedPublicPort(QuerySslPort).ToString());
            }

            if (enabledServices.Contains(CouchbaseService.Search))
            {
                content.Add("fts", container.GetMappedPublicPort(SearchPort).ToString());
                content.Add("ftsSSL", container.GetMappedPublicPort(SearchSslPort).ToString());
            }

            if (enabledServices.Contains(CouchbaseService.Analytics))
            {
                content.Add("cbas", container.GetMappedPublicPort(AnalyticsPort).ToString());
                content.Add("cbasSSL", container.GetMappedPublicPort(AnalyticsSslPort).ToString());
            }

            if (enabledServices.Contains(CouchbaseService.Eventing))
            {
                content.Add("eventingAdminPort", container.GetMappedPublicPort(EventingPort).ToString());
                content.Add("eventingSSL", container.GetMappedPublicPort(EventingSslPort).ToString());
            }

            Content = new FormUrlEncodedContent(content);
        }
    }

    /// <summary>
    /// An HTTP request that sets the Couchbase administrator credentials.
    /// </summary>
    private sealed class SetupCredentialsRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetupCredentialsRequest" /> class.
        /// <remarks>
        /// https://docs.couchbase.com/server/current/rest-api/rest-establish-credentials.html#http-method-and-uri.
        /// </remarks>
        /// </summary>
        public SetupCredentialsRequest()
            : base(HttpMethod.Post, "/settings/web")
        {
            IDictionary<string, string> content = new Dictionary<string, string>();
            content.Add("username", DefaultUsername);
            content.Add("password", DefaultPassword);
            content.Add("port", "SAME");
            Content = new FormUrlEncodedContent(content);
        }
    }

    /// <summary>
    /// An HTTP request that creates a Couchbase bucket.
    /// </summary>
    private sealed class CreateBucketRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateBucketRequest" /> class.
        /// </summary>
        /// <remarks>
        /// https://docs.couchbase.com/server/current/rest-api/rest-bucket-create.html#http-methods-and-uris.
        /// </remarks>
        /// <param name="bucket">The Couchbase bucket.</param>
        public CreateBucketRequest(CouchbaseBucket bucket)
            : base(HttpMethod.Post, "/pools/default/buckets")
        {
            IDictionary<string, string> content = new Dictionary<string, string>();
            content.Add("name", bucket.Name);
            content.Add("flushEnabled", bucket.FlushEnabled ? "1" : "0");
            content.Add("ramQuota", bucket.QuotaMiB.ToString());
            content.Add("replicaNumber", bucket.ReplicaNumber.ToString());
            Content = new FormUrlEncodedContent(content);
        }
    }

    /// <summary>
    /// An HTTP retry handler that sends an HTTP request until it succeeds.
    /// </summary>
    /// <remarks>
    /// Sending an HTTP request to Couchbase's API sometimes fails with the following
    /// error: System.Net.Http.HttpIOException: The response ended prematurely (ResponseEnded).
    /// The HTTP status code 504 indicates an issue with the Couchbase backend.
    /// It is likely that the API is not yet ready to process HTTP requests.
    /// Typically, trying it again resolves the issue.
    /// </remarks>
    private sealed class RetryHandler : DelegatingHandler
    {
        private const int MaxRetries = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryHandler" /> class.
        /// </summary>
        public RetryHandler()
            : base(new HttpClientHandler())
        {
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            for (var _ = 0; _ < MaxRetries; _++)
            {
                try
                {
                    return await base.SendAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (HttpRequestException)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            throw new HttpRequestException($"Unable to configure Couchbase. The HTTP request '{request.RequestUri}' did not complete successfully.");
        }
    }
}