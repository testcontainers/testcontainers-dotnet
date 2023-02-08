namespace Testcontainers.Minio;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MinioContainer : DockerContainer
{
    private readonly MinioConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public MinioContainer(MinioConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the AWS access key id.
    /// </summary>
    /// <returns>The AWS access key id.</returns>
    public string GetAccessKeyId()
    {
        return _configuration.Username;
    }

    /// <summary>
    /// Gets the AWS access secret.
    /// </summary>
    /// <returns>The AWS access secret.</returns>
    public string GetAccessSecret()
    {
        return _configuration.Password;
    }

    /// <summary>
    /// Gets the Mino endpoint.
    /// </summary>
    /// <returns>The Mino endpoint.</returns>
    public string GetMinoEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(MinioBuilder.MinioPort)).ToString();
    }
}