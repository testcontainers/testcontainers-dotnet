namespace Testcontainers.Chroma;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ChromaContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChromaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ChromaContainer(ChromaConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the base address of the Chroma HTTP API.
    /// </summary>
    /// <returns>The base address of the Chroma HTTP API, like <c>http://localhost:32768/</c>.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(ChromaBuilder.ChromaHttpPort)).ToString();
    }
}
