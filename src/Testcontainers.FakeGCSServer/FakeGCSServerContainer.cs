namespace Testcontainers.FakeGCSServer;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class FakeGCSServerContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGCSServerContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public FakeGCSServerContainer(FakeGCSServerConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the FakeGCSServer connection string.
    /// </summary>
    /// <returns>The FakeGCSServer connection string.</returns>
    public string GetConnectionString()
    {
        var builder = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(FakeGCSServerBuilder.FakeGCSServerPort), "storage/v1/");
        return builder.ToString();
    }
}