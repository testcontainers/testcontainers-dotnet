namespace Testcontainers.DynamoDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class DynamoDbContainer : DockerContainer
{
    private readonly DynamoDbConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DynamoDbContainer(DynamoDbConfiguration configuration, ILogger logger)
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
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(DynamoDbBuilder.DynalitePort)).ToString();
    }
}