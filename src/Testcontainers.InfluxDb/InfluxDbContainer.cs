namespace Testcontainers.InfluxDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class InfluxDbContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InfluxDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public InfluxDbContainer(InfluxDbConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the InfluxDb address.
    /// </summary>
    /// <returns>The InfluxDb address.</returns>
    public string GetAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(InfluxDbBuilder.InfluxDbPort)).ToString();
    }
}