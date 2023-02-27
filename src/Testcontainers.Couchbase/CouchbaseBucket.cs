namespace Testcontainers.Couchbase;

/// <summary>
/// A Couchbase bucket.
/// </summary>
public sealed class CouchbaseBucket
{
    /// <summary>
    /// Gets the default bucket.
    /// </summary>
    public static readonly CouchbaseBucket Default = new CouchbaseBucket(Guid.NewGuid().ToString("D"));

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseBucket" /> struct.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="flushEnabled">A value indicating whether flush is enabled or not.</param>
    /// <param name="primaryIndexEnabled">A value indicating whether primary index is enabled or not.</param>
    /// <param name="quotaMiB">The quota in MiB.</param>
    /// <param name="replicaNumber">The replica number.</param>
    private CouchbaseBucket(
        string name,
        bool flushEnabled = false,
        bool primaryIndexEnabled = true,
        ushort quotaMiB = 100,
        ushort replicaNumber = 0)
    {
        Name = name;
        FlushEnabled = flushEnabled;
        PrimaryIndexEnabled = primaryIndexEnabled;
        QuotaMiB = quotaMiB;
        ReplicaNumber = replicaNumber;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a value indicating whether flush is enabled or not.
    /// </summary>
    public bool FlushEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether primary index is enabled or not.
    /// </summary>
    public bool PrimaryIndexEnabled { get; }

    /// <summary>
    /// Gets the quota in MiB.
    /// </summary>
    public ushort QuotaMiB { get; }

    /// <summary>
    /// Gets the replica number.
    /// </summary>
    public ushort ReplicaNumber { get; }
}