namespace Testcontainers.Temporalio;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class TemporalContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public TemporalContainer(TemporalConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Temporal gRPC endpoint for SDK clients and workers.
    /// </summary>
    /// <remarks>
    /// The Temporal .NET SDK expects <c>host:port</c> without a protocol scheme.
    /// Using a URI like <c>http://host:port</c> will throw an <see cref="ArgumentException" />.
    /// <para>
    /// Usage example:
    /// <code>
    /// var client = await TemporalClient.ConnectAsync(
    ///     new("localhost:7233") { Namespace = "default" });
    /// </code>
    /// </para>
    /// <seealso href="https://github.com/temporalio/sdk-dotnet?tab=readme-ov-file#running-a-worker"/>
    /// </remarks>
    /// <returns>The Temporal gRPC endpoint in <c>host:port</c> format.</returns>
    public string GetGrpcAddress()
    {
        return Hostname + ":" + GetMappedPublicPort(TemporalBuilder.TemporalGrpcPort);
    }

    /// <summary>
    /// Gets the Temporal Web UI address.
    /// </summary>
    /// <returns>The Temporal Web UI base address.</returns>
    public string GetWebUiAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(TemporalBuilder.TemporalHttpPort)).ToString();
    }
}
