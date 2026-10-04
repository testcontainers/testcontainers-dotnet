namespace Testcontainers.RustFs;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class RustFsContainer : DockerContainer
{
    private readonly RustFsConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="RustFsContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public RustFsContainer(RustFsConfiguration configuration)
        : base(configuration)
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
    /// Gets the RustFS connection string.
    /// </summary>
    /// <returns>The RustFS connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(RustFsBuilder.RustFsPort)).ToString();
    }
}