namespace Testcontainers.Pulsar;

internal readonly struct PulsarService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarService" /> struct.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    private PulsarService(string identifier)
    {
        Identifier = identifier;
    }
    
    /// <summary>
    /// Gets the Auth.
    /// </summary>
    public static readonly PulsarService Authentication = new PulsarService("auth");
    
    /// <summary>
    /// Gets the Function worker service.
    /// </summary>
    public static readonly PulsarService FunctionWorker = new PulsarService("funk");
    
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public string Identifier { get; }
}


