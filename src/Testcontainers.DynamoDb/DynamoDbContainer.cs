namespace Testcontainers.DynamoDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class DynamoDbContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DynamoDbContainer(DynamoDbConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the DynamoDb connection string.
    /// </summary>
    /// <returns>The DynamoDb connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(DynamoDbBuilder.DynamoDbPort)).ToString();
    }
}