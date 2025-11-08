namespace Testcontainers.Mosquitto;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MosquittoContainer : DockerContainer
{
    private readonly MosquittoConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MosquittoContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MosquittoContainer(MosquittoConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the MQTT endpoint.
    /// </summary>
    /// <returns>A TCP address in the format: <c>tcp://hostname:port</c>.</returns>
    public string GetEndpoint()
    {
        return new UriBuilder("mqtt", Hostname, GetMappedPublicPort(MosquittoBuilder.MqttPort)).ToString();
    }

    /// <summary>
    /// Gets the secure MQTT endpoint.
    /// </summary>
    /// <returns>A TCP address in the format: <c>tcp://hostname:port</c>.</returns>
    public string GetSecureEndpoint()
    {
        ThrowIfTlsNotEnabled();
        return new UriBuilder("mqtts", Hostname, GetMappedPublicPort(MosquittoBuilder.MqttTlsPort)).ToString();
    }

    /// <summary>
    /// Gets the WebSocket endpoint.
    /// </summary>
    /// <returns>A WS address in the format: <c>ws://hostname:port</c>.</returns>
    public string GetWsEndpoint()
    {
        return new UriBuilder("ws", Hostname, GetMappedPublicPort(MosquittoBuilder.MqttWsPort)).ToString();
    }

    /// <summary>
    /// Gets the secure WebSocket endpoint.
    /// </summary>
    /// <returns>A WS address in the format: <c>wss://hostname:port</c>.</returns>
    public string GetWssEndpoint()
    {
        ThrowIfTlsNotEnabled();
        return new UriBuilder("wss", Hostname, GetMappedPublicPort(MosquittoBuilder.MqttWssPort)).ToString();
    }

    /// <summary>
    /// Throws <see cref="InvalidOperationException" /> when TLS/SSL is not enabled in the configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">TLS/SSL is not enabled in the configuration.</exception>
    private void ThrowIfTlsNotEnabled()
    {
        if (!_configuration.TlsEnabled)
        {
            throw new InvalidOperationException("TLS/SSL support is not enabled in the container configuration.");
        }
    }
}