using System;

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

    public string GetServerApiUrl()
    {
        return new UriBuilder("http", Hostname, GetMappedPublicPort(SeqBuilder.SeqApiPort)).Uri.ToString();
    }
}