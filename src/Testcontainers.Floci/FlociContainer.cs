namespace Testcontainers.Floci;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class FlociContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlociContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public FlociContainer(FlociConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Floci connection string.
    /// </summary>
    /// <returns>The Floci connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(FlociBuilder.FlociPort)).ToString();
    }
}