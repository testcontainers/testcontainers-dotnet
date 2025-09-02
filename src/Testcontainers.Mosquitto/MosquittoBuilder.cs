namespace TestContainers.Mosquitto;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public class MosquittoBuilder : ContainerBuilder<MosquittoBuilder, MosquittoContainer, MosquittoConfiguration>
{
	public const string MosquittoImage = "eclipse-mosquitto:2.0";

	public const int TcpPort = 1883;
	public const int TlsPort = 8883;
	public const int WsPort = 80;
	public const int WssPort = 443;
	public const string CertificateFilePath = "/mosquitto/certs/server.pem";
	public const string CertificateKeyFilePath = "/mosquitto/certs/server-key.pem";

	/// <summary>
	/// Initializes a new instance of the <see cref="MosquittoBuilder" /> class.
	/// </summary>
	public MosquittoBuilder()
	  : this(new MosquittoConfiguration())
	{
		DockerResourceConfiguration = Init().DockerResourceConfiguration;
	}

	public MosquittoBuilder(MosquittoConfiguration resourceConfiguration)
		: base(resourceConfiguration)
	{
		DockerResourceConfiguration = resourceConfiguration;
	}

	/// <inheritdoc />
	protected override MosquittoConfiguration DockerResourceConfiguration { get; }

	/// <inheritdoc />
	public override MosquittoContainer Build()
	{
		Validate();

		var sb = new StringBuilder();
		sb.AppendUnixLine("per_listener_settings true");

		sb.AppendUnixLine();
		sb.AppendUnixLine("# MQTT listener");
		sb.AppendUnixLine($"listener {TcpPort}");
		sb.AppendUnixLine("protocol mqtt");
		sb.AppendUnixLine("allow_anonymous true");

		sb.AppendUnixLine();
		sb.AppendUnixLine("# WebSocket listener");
		sb.AppendUnixLine($"listener {WsPort}");
		sb.AppendUnixLine("protocol websockets");
		sb.AppendUnixLine("allow_anonymous true");

		if (DockerResourceConfiguration.HasCertificate)
		{
			sb.AppendUnixLine();
			sb.AppendUnixLine("# MQTT listener (encrypted)");
			sb.AppendUnixLine($"listener {TlsPort}");
			sb.AppendUnixLine("protocol mqtt");
			sb.AppendUnixLine("allow_anonymous true");
			sb.AppendUnixLine($"certfile {CertificateFilePath}");
			sb.AppendUnixLine($"keyfile {CertificateKeyFilePath}");

			sb.AppendUnixLine();
			sb.AppendUnixLine("# WebSocket listener (encrypted)");
			sb.AppendUnixLine($"listener {WssPort}");
			sb.AppendUnixLine("protocol websockets");
			sb.AppendUnixLine("allow_anonymous true");
			sb.AppendUnixLine($"certfile {CertificateFilePath}");
			sb.AppendUnixLine($"keyfile {CertificateKeyFilePath}");
		}

		var config = Clone(DockerResourceConfiguration)
			.WithResourceMapping(Encoding.UTF8.GetBytes(sb.ToString()), "/mosquitto/config/mosquitto.conf");

		return new MosquittoContainer(config.DockerResourceConfiguration);
	}


	public MosquittoBuilder WithCertificate(string certificate, string certificateKey)
	{
		return Merge(DockerResourceConfiguration, new MosquittoConfiguration(certificate: certificate, certificateKey: certificateKey))
			.WithPortBinding(TlsPort, true)
			.WithPortBinding(WssPort, true)
			.WithResourceMapping(Encoding.UTF8.GetBytes(certificate), CertificateFilePath)
			.WithResourceMapping(Encoding.UTF8.GetBytes(certificateKey), CertificateKeyFilePath);
	}

	/// <inheritdoc />
	protected override MosquittoBuilder Init()
	{
		var builder = base.Init()
			.WithImage(MosquittoImage)
			.WithPortBinding(TcpPort, true)
			.WithPortBinding(WsPort, true)
			.WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(@"mosquitto.*running"));

		return builder;
	}

	/// <inheritdoc />
	protected override void Validate()
	{
		base.Validate();

		_ = Guard.Argument(DockerResourceConfiguration, "Certificate")
			.ThrowIf(argument => 1.Equals(new[] { argument.Value.Certificate, argument.Value.CertificateKey }.Count(string.IsNullOrWhiteSpace)), argument => new ArgumentException($"Both {nameof(argument.Value.Certificate)} and {nameof(argument.Value.CertificateKey)} must be supplied if one is.", argument.Name));
	}

	/// <inheritdoc />
	protected override MosquittoBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
	{
		return Merge(DockerResourceConfiguration, new MosquittoConfiguration(resourceConfiguration));
	}

	/// <inheritdoc />
	protected override MosquittoBuilder Clone(IContainerConfiguration resourceConfiguration)
	{
		return Merge(DockerResourceConfiguration, new MosquittoConfiguration(resourceConfiguration));
	}

	/// <inheritdoc />
	protected override MosquittoBuilder Merge(MosquittoConfiguration oldValue, MosquittoConfiguration newValue)
	{
		return new MosquittoBuilder(new MosquittoConfiguration(oldValue, newValue));
	}
}
