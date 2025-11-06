namespace Testcontainers.Playwright;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class PlaywrightContainer : DockerContainer
{
    private readonly PlaywrightConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaywrightContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public PlaywrightContainer(PlaywrightConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Playwright connection string.
    /// </summary>
    /// <returns>The Playwright connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder("ws", Hostname, GetMappedPublicPort(PlaywrightBuilder.PlaywrightPort), "/playwright").ToString();
    }

    /// <summary>
    /// Gets the Playwright network.
    /// </summary>
    /// <returns>The Playwright network.</returns>
    public INetwork GetNetwork()
    {
        return _configuration.Networks.Single();
    }
}