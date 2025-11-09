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
    /// Gets the MQTT port.
    /// </summary>
    public ushort MqttPort => GetMappedPublicPort(MosquittoBuilder.MqttPort);

    /// <summary>
    /// Gets the secure MQTT port.
    /// </summary>
    public ushort MqttTlsPort => GetMappedPublicPort(MosquittoBuilder.MqttTlsPort);

    /// <summary>
    /// Gets the MQTT connection string.
    /// </summary>
    /// <returns>An MQTT connection string in the format: <c>mqtt://hostname:port</c>.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder("mqtt", Hostname, MqttPort).ToString();
    }

    /// <summary>
    /// Gets the secure MQTT connection string.
    /// </summary>
    /// <returns>A secure MQTT connection string in the format: <c>mqtts://hostname:port</c>.</returns>
    /// <exception cref="InvalidOperationException">TLS/SSL support is not enabled in the container configuration.</exception>
    public string GetSecureConnectionString()
    {
        ThrowIfTlsNotEnabled();
        return new UriBuilder("mqtts", Hostname, MqttTlsPort).ToString();
    }

    /// <summary>
    /// Gets the MQTT over WebSocket connection string.
    /// </summary>
    /// <returns>A WebSocket connection string in the format: <c>ws://hostname:port</c>.</returns>
    public string GetWsConnectionString()
    {
        return new UriBuilder("ws", Hostname, GetMappedPublicPort(MosquittoBuilder.MqttWsPort)).ToString();
    }

    /// <summary>
    /// Gets the secure MQTT over WebSocket connection string.
    /// </summary>
    /// <returns>A secure WebSocket connection string in the format: <c>wss://hostname:port</c>.</returns>
    /// <exception cref="InvalidOperationException">TLS/SSL support is not enabled in the container configuration.</exception>
    public string GetWssConnectionString()
    {
        ThrowIfTlsNotEnabled();
        return new UriBuilder("wss", Hostname, GetMappedPublicPort(MosquittoBuilder.MqttWssPort)).ToString();
    }

    /// <summary>
    /// Throws <see cref="InvalidOperationException" /> when TLS/SSL support is not enabled in the container configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">TLS/SSL support is not enabled in the container configuration.</exception>
    private void ThrowIfTlsNotEnabled()
    {
        if (!_configuration.TlsEnabled)
        {
            throw new InvalidOperationException("TLS/SSL support is not enabled in the container configuration.");
        }
    }
}