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
    /// Gets the Temporal gRPC address.
    /// </summary>
    /// <remarks>
    /// The Temporal SDK (client library) expects <c>host:port</c> without a scheme.
    /// </remarks>
    /// <example>
    ///   <code>
    ///    var clientOptions = new TemporalClientConnectOptions();
    ///    clientOptions.TargetHost = temporalContainer.GetGrpcAddress();
    ///   <br />
    ///    var connectedClient = await TemporalClient.ConnectAsync(clientOptions);
    ///   </code>
    /// </example>
    /// <seealso href="https://github.com/temporalio/sdk-dotnet?tab=readme-ov-file#running-a-worker" />
    /// <returns>The Temporal gRPC address in <c>host:port</c> format.</returns>
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