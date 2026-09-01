namespace Testcontainers.FlociAz;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class FlociAzContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlociAzContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public FlociAzContainer(FlociAzConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the FlociAz storage connection string.
    /// </summary>
    /// <returns>The FlociAz storage connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("DefaultEndpointsProtocol", Uri.UriSchemeHttp);
        properties.Add("AccountName", FlociAzBuilder.AccountName);
        properties.Add("AccountKey", FlociAzBuilder.AccountKey);
        properties.Add("BlobEndpoint", GetServiceEndpoint());
        properties.Add("QueueEndpoint", GetServiceEndpoint("queue"));
        properties.Add("TableEndpoint", GetServiceEndpoint("table"));
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Gets the FlociAz endpoint used by root-level and Azure Resource Manager APIs.
    /// </summary>
    /// <returns>The FlociAz endpoint.</returns>
    public string GetEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(FlociAzBuilder.FlociAzPort)).ToString();
    }

    /// <summary>
    /// Gets the FlociAz service endpoint.
    /// </summary>
    /// <remarks>
    /// FlociAz routes its REST services by path: the blob storage service uses the
    /// bare <c>/{accountName}</c> path (omit the <paramref name="service" /> argument),
    /// other services use <c>/{accountName}-{service}</c> (e.g., <c>queue</c>,
    /// <c>table</c>, <c>cosmos</c>, <c>keyvault</c>, <c>appconfig</c>, <c>functions</c>).
    /// </remarks>
    /// <param name="service">The service name, or <c>null</c> for the blob storage endpoint.</param>
    /// <returns>The FlociAz service endpoint.</returns>
    public string GetServiceEndpoint(string service = null)
    {
        var path = string.IsNullOrEmpty(service) ? FlociAzBuilder.AccountName : string.Join("-", FlociAzBuilder.AccountName, service);
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(FlociAzBuilder.FlociAzPort), path + "/").ToString();
    }

    /// <summary>
    /// Gets the FlociAz Cosmos DB connection string.
    /// </summary>
    /// <returns>The FlociAz Cosmos DB connection string.</returns>
    public string GetCosmosConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("AccountEndpoint", GetServiceEndpoint("cosmos"));
        properties.Add("AccountKey", FlociAzBuilder.AccountKey);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }
}
