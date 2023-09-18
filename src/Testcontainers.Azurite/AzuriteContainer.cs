namespace Testcontainers.Azurite;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class AzuriteContainer : DockerContainer
{
    private const string AccountName = "devstoreaccount1";

    private const string AccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public AzuriteContainer(AzuriteConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the Azurite connection string.
    /// </summary>
    /// <returns>The Azurite connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("DefaultEndpointsProtocol", Uri.UriSchemeHttp);
        properties.Add("AccountName", AccountName);
        properties.Add("AccountKey", AccountKey);
        properties.Add("BlobEndpoint", GetBlobEndpoint());
        properties.Add("QueueEndpoint", GetQueueEndpoint());
        properties.Add("TableEndpoint", GetTableEndpoint());
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Gets the Azurite blob endpoint.
    /// </summary>
    /// <returns>The Azurite blob endpoint.</returns>
    public string GetBlobEndpoint() => new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AzuriteBuilder.BlobPort), AccountName).ToString();

    /// <summary>
    /// Gets the Azurite queue endpoint.
    /// </summary>
    /// <returns>The Azurite queue endpoint.</returns>
    public string GetQueueEndpoint() => new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AzuriteBuilder.QueuePort), AccountName).ToString();

    /// <summary>
    /// Gets Azurite table endpoint.
    /// </summary>
    /// <returns>The Azurite table endpoint.</returns>
    public string GetTableEndpoint() => new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AzuriteBuilder.TablePort), AccountName).ToString();
}