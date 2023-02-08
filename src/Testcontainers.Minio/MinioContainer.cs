namespace Testcontainers.Minio;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MinioContainer: DockerContainer
{
    private readonly MinioConfiguration _configuration;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MinioContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public MinioContainer(MinioConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Minio UserName.
    /// </summary>
    /// <returns>The Minio UserName.</returns>
    public string GetUserName()
    {
        return _configuration.UserName;
    }
    
    /// <summary>
    /// Gets the Minio Password.
    /// </summary>
    /// <returns>The Minio Password.</returns>
    public string GetPassword()
    {
        return _configuration.Password;
    }
    
    /// <summary>
    /// Gets the Minio AccessKeyid for the AmazonS3 purpose.
    /// </summary>
    /// <returns>The Minio AccessKeyid.</returns>
    public string GetAccessId()
    {
        return _configuration.UserName;
    }
    
    /// <summary>
    /// Gets the Minio AccessKeySecret for the AmazonS3 purpose.
    /// </summary>
    /// <returns>The Minio AccessKeySecret.</returns>
    public string GetAccessKey()
    {
        return _configuration.Password;
    }
    
    /// <summary>
    /// Gets the Minio Url.
    /// </summary>
    /// <returns>The Minio Url.</returns>
    public string GetMinioUrl()
    {
        var port = GetMappedPublicPort(MinioBuilder.MinioPort);
        return $"http://{Hostname}:{port}";
    }
}