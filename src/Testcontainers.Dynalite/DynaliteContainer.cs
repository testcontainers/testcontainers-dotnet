namespace Testcontainers.Dynalite;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class DynaliteContainer : DockerContainer
{
    private readonly DynaliteConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynaliteContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DynaliteContainer(DynaliteConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }
    
    /// <summary>
    /// Gets the Dynalite endpoint.
    /// </summary>
    /// <returns>The Dynalite endpoint.</returns>
    public string GetEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(DynaliteBuilder.DynalitePort)).ToString();
    }
    
    
}