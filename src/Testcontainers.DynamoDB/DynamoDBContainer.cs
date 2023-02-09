namespace Testcontainers.DynamoDB;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class DynamoDBContainer : DockerContainer
{
    private readonly DynamoDBConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDBContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DynamoDBContainer(DynamoDBConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }
    
    /// <summary>
    /// Gets the DynamoDB endpoint.
    /// </summary>
    /// <returns>The DynamoDB endpoint.</returns>
    private string GetEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(DynamoDBBuilder.DynamoDbPort)).ToString();
    }

    public AmazonDynamoDBClient GetAmazonDynamoDBClient()
    {
        var clientConfig = new AmazonDynamoDBConfig();
        clientConfig.ServiceURL = this.GetEndpoint();
        clientConfig.UseHttp = true;
        return new AmazonDynamoDBClient(new BasicAWSCredentials("dummy", "dummy"), clientConfig);
    }
}