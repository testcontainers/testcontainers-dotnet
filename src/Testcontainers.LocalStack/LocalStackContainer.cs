namespace Testcontainers.LocalStack;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class LocalStackContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public LocalStackContainer(LocalStackConfiguration configuration)
        : base(configuration) { }

    /// <summary>
    /// Gets the LocalStack connection string.
    /// </summary>
    /// <returns>The LocalStack connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(LocalStackBuilder.LocalStackPort)
        ).ToString();
    }
}
