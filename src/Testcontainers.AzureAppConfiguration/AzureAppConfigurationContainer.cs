namespace Testcontainers.AzureAppConfiguration;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class AzureAppConfigurationContainer : DockerContainer
{
    private readonly AzureAppConfigurationConfiguration _configuration;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public AzureAppConfigurationContainer(AzureAppConfigurationConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Azure App Configuration connection string.
    /// </summary>
    /// <returns>The Azure App Configuration connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Endpoint", new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AzureAppConfigurationBuilder.AzureAppConfigurationPort)).ToString());
        properties.Add("Id", _configuration.Credential);
        properties.Add("Secret", _configuration.Secret);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }
}