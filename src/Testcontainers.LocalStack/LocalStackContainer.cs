namespace Testcontainers.Minio;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class LocalStackContainer : DockerContainer
{
    private readonly LocalStackConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public LocalStackContainer(LocalStackConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the AWS access key id.
    /// </summary>
    /// <returns>The AWS access key id.</returns>
    public string GetAccessKeyId() => "dummy";

    /// <summary>
    /// Gets the AWS access secret.
    /// </summary>
    /// <returns>The AWS access secret.</returns>
    public string GetAccessSecret() => "dummy";
    

    /// <summary>
    /// Gets the Minio endpoint.
    /// </summary>
    /// <returns>The LocalStack endpoint.</returns>
    public string GetEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(LocalStackBuilder.LocalStackPort)).ToString();
    }
}