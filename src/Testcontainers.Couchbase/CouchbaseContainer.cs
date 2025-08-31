namespace Testcontainers.Couchbase;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class CouchbaseContainer : DockerContainer
{
    private readonly CouchbaseConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public CouchbaseContainer(CouchbaseConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
        Starting += (_, _) =>
            Logger.LogInformation("Couchbase container is starting, performing configuration.");
        Started += (_, _) =>
            Logger.LogInformation(
                "Couchbase container is ready! UI available at {Url}.",
                new UriBuilder(
                    Uri.UriSchemeHttp,
                    Hostname,
                    GetMappedPublicPort(CouchbaseBuilder.MgmtPort)
                )
            );
    }

    /// <summary>
    /// Gets a list of buckets.
    /// </summary>
    public IEnumerable<CouchbaseBucket> Buckets => _configuration.Buckets;

    /// <summary>
    /// Gets the Couchbase connection string.
    /// </summary>
    /// <returns>The Couchbase connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder("couchbase", Hostname, GetMappedPublicPort(CouchbaseBuilder.KvPort))
            .Uri
            .Authority;
    }
}
