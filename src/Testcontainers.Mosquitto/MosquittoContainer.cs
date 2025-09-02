namespace TestContainers.Mosquitto;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MosquittoContainer : DockerContainer
{
	private readonly bool _isSecure;

	/// <summary>
	/// Initializes a new instance of the <see cref="MosquittoContainer" /> class.
	/// </summary>
	/// <param name="configuration">The container configuration.</param>
	public MosquittoContainer(MosquittoConfiguration configuration)
		  : base(configuration)
	{
		_isSecure = configuration.HasCertificate;
	}

	/// <summary>
	/// Gets the MQTT endpoint.
	/// </summary>
	/// <returns>A TCP address in the format: <c>tcp://hostname:port</c>.</returns>
	public string GetEndpoint()
	{
		return new UriBuilder(Uri.UriSchemeNetTcp, Hostname, GetPort()).ToString();
	}

	/// <summary>
	/// Gets the MQTT endpoint port.
	/// </summary>
	/// <returns>Exposed port for insecure MQQT connections.</returns>
	public ushort GetPort()
	{
		return GetMappedPublicPort(MosquittoBuilder.TcpPort);
	}

	/// <summary>
	/// Gets the secure MQTT endpoint.
	/// </summary>
	/// <returns>A TCP address in the format: <c>tcp://hostname:port</c>.</returns>
	public string GetSecureEndpoint()
	{
		ThrowIfNotSecure();
		return new UriBuilder(Uri.UriSchemeNetTcp, Hostname, GetMappedPublicPort(MosquittoBuilder.TlsPort)).ToString();
	}

	/// <summary>
	/// Gets the secure MQTT endpoint port.
	/// </summary>
	/// <returns>Exposed port for secure MQTT connections.</returns>
	public ushort GetSecurePort()
	{
		return GetMappedPublicPort(MosquittoBuilder.TlsPort);
	}

	/// <summary>
	/// Gets the WebSocket endpoint.
	/// </summary>
	/// <returns>A WS address in the format: <c>ws://hostname:port</c>.</returns>
	public string GetWsEndpoint()
	{
		return new UriBuilder("ws", Hostname, GetMappedPublicPort(MosquittoBuilder.WsPort)).ToString();
	}

	/// <summary>
	/// Gets the secure WebSocket endpoint.
	/// </summary>
	/// <returns>A WS address in the format: <c>ws://hostname:port</c>.</returns>
	public string GetWssEndpoint()
	{
		ThrowIfNotSecure();
		return new UriBuilder("wss", Hostname, GetMappedPublicPort(MosquittoBuilder.WssPort)).ToString();
	}

	private void ThrowIfNotSecure()
	{
		if (_isSecure)
		{
			return;
		}

		throw new InvalidOperationException("The container was not configured with TLS/SSL support.");
	}
}
