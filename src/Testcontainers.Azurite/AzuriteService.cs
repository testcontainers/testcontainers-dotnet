namespace Testcontainers.Azurite;

/// <summary>
/// An Azurite service.
/// </summary>
internal readonly record struct AzuriteService
{
    /// <summary>
    /// Gets the Blob service.
    /// </summary>
    public static readonly AzuriteService Blob = new AzuriteService("blob");

    /// <summary>
    /// Gets the Queue service.
    /// </summary>
    public static readonly AzuriteService Queue = new AzuriteService("queue");

    /// <summary>
    /// Gets the Table service.
    /// </summary>
    public static readonly AzuriteService Table = new AzuriteService("table");

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteService" /> struct.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    private AzuriteService(string identifier)
    {
        _ = identifier;
    }
}