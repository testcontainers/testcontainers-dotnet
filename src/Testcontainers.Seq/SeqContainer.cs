namespace Testcontainers.Seq;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class SeqContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public SeqContainer(SeqConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Seq endpoint.
    /// </summary>
    /// <returns>The Seq endpoint.</returns>
    public string GetEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(SeqBuilder.SeqPort)).ToString();
    }
}