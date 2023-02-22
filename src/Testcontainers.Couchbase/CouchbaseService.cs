namespace Testcontainers.Couchbase;

/// <summary>
/// A Couchbase service.
/// </summary>
internal readonly struct CouchbaseService
{
    /// <summary>
    /// Gets the Data service.
    /// </summary>
    public static readonly CouchbaseService Data = new("kv", 256);

    /// <summary>
    /// Gets the Index service.
    /// </summary>
    public static readonly CouchbaseService Index = new("index", 256);

    /// <summary>
    /// Gets the Query service.
    /// </summary>
    public static readonly CouchbaseService Query = new("n1ql", 0);

    /// <summary>
    /// Gets the Search service.
    /// </summary>
    public static readonly CouchbaseService Search = new("fts", 256);

    /// <summary>
    /// Gets the Analytics service.
    /// </summary>
    public static readonly CouchbaseService Analytics = new("cbas", 1024);

    /// <summary>
    /// Gets the Eventing service.
    /// </summary>
    public static readonly CouchbaseService Eventing = new("eventing", 256);

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseService" /> struct.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <param name="minimumQuotaMb">The minimum quota in MB.</param>
    private CouchbaseService(string identifier, ushort minimumQuotaMb)
    {
        Identifier = identifier;
        MinimumQuotaMb = minimumQuotaMb;
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Gets the minimum quota in MB.
    /// </summary>
    public ushort MinimumQuotaMb { get; }

    /// <summary>
    /// Gets a value indicating whether the service has a minimum quota or not.
    /// </summary>
    public bool HasQuota => MinimumQuotaMb > 0;
}