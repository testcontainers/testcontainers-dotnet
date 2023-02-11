namespace Testcontainers.MongoDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MongoDbContainer : DockerContainer
{
    private readonly MongoDbConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public MongoDbContainer(MongoDbConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the MongoDb connection string.
    /// </summary>
    /// <returns>The MongoDb connection string.</returns>
    public string GetConnectionString()
    {
        // The MongoDb documentation recommends to use percent-encoding for username and password: https://www.mongodb.com/docs/manual/reference/connection-string/.
        var endpoint = new UriBuilder("mongodb://", Hostname, GetMappedPublicPort(MongoDbBuilder.MongoDbPort));
        endpoint.UserName = Uri.EscapeDataString(_configuration.Username);
        endpoint.Password = Uri.EscapeDataString(_configuration.Password);
        return endpoint.ToString();
    }
}