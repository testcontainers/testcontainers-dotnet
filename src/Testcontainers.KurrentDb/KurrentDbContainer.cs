namespace Testcontainers.KurrentDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class KurrentDbContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public KurrentDbContainer(KurrentDbConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the KurrentDb connection string.
    /// </summary>
    /// <returns>The KurrentDb connection string.</returns>
    public string GetConnectionString()
    {
        var endpoint =
            new UriBuilder("kurrentdb", Hostname, GetMappedPublicPort(KurrentDbBuilder.KurrentDbPort))
            {
                Query = "tls=false",
            };
        return endpoint.ToString();
    }
}