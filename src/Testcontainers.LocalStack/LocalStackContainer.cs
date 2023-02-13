namespace Testcontainers.LocalStack;

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
    /// Gets the LocalStack connection string.
    /// </summary>
    /// <returns>The LocalStack connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(LocalStackBuilder.LocalStackPort)).ToString();
    }
}