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
    /// Gets the AWS access key.
    /// </summary>
    /// <returns>The AWS access key.</returns>
    public string GetAccessKey()
    {
        return _configuration.Username;
    }

    /// <summary>
    /// Gets the AWS secret key.
    /// </summary>
    /// <returns>The AWS secret key.</returns>
    public string GetSecretKey()
    {
        return _configuration.Password;
    }

    /// <summary>
    /// Gets the Minio connection string.
    /// </summary>
    /// <returns>The Minio connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(MinioBuilder.MinioPort)).ToString();
    }
}