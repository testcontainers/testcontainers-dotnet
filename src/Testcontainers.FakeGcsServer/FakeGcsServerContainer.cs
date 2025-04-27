namespace Testcontainers.FakeGcsServer;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class FakeGcsServerContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public FakeGcsServerContainer(FakeGcsServerConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the FakeGcsServer connection string.
    /// </summary>
    /// <returns>The FakeGcsServer connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(FakeGcsServerBuilder.FakeGcsServerPort), "/storage/v1/").ToString();
    }
}