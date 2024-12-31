using Microsoft.Azure.Cosmos;

namespace Testcontainers.CosmosDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class CosmosDbContainer : DockerContainer
{
    private readonly int _port;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public CosmosDbContainer(CosmosDbConfiguration configuration)
        : base(configuration)
    {
        _port = int.Parse(configuration.PortBindings.First().Value);
    }

    /// <summary>
    /// Gets the CosmosDb connection string.
    /// </summary>
    /// <returns>The CosmosDb connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("AccountEndpoint", new UriBuilder(Uri.UriSchemeHttp, Hostname, _port).ToString());
        properties.Add("AccountKey", CosmosDbBuilder.DefaultAccountKey);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Gets a configured cosmos client with connection mode set to Gateway.
    /// </summary>
    public CosmosClient CosmosClient => new(GetConnectionString(), new() { ConnectionMode = ConnectionMode.Gateway });

}
