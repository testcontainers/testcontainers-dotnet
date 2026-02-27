namespace Testcontainers.Azurite;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class AzuriteContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public AzuriteContainer(AzuriteConfiguration configuration)
        : base(configuration)
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
        properties.Add("AccountName", AzuriteBuilder.AccountName);
        properties.Add("AccountKey", AzuriteBuilder.AccountKey);
        properties.Add("BlobEndpoint", GetBlobEndpoint());
        properties.Add("QueueEndpoint", GetQueueEndpoint());
        properties.Add("TableEndpoint", GetTableEndpoint());
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Gets the blob endpoint
    /// </summary>
    /// <returns>The Azurite blob endpoint</returns>
    public string GetBlobEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AzuriteBuilder.BlobPort), AzuriteBuilder.AccountName).ToString();
    }

    /// <summary>
    /// Gets the queue endpoint
    /// </summary>
    /// <returns>The Azurite queue endpoint</returns>
    public string GetQueueEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AzuriteBuilder.QueuePort), AzuriteBuilder.AccountName).ToString();
    }

    /// <summary>
    /// Gets the table endpoint
    /// </summary>
    /// <returns>The Azurite table endpoint</returns>
    public string GetTableEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AzuriteBuilder.TablePort), AzuriteBuilder.AccountName).ToString();
    }
}