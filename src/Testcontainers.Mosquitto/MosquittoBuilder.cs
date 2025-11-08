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

		var sb = new StringWriter();
		sb.NewLine = "\n";
		sb.WriteLine("per_listener_settings true");

		sb.WriteLine();
		sb.WriteLine("# MQTT listener");
		sb.WriteLine($"listener {TcpPort}");
		sb.WriteLine("protocol mqtt");
		sb.WriteLine("allow_anonymous true");

		sb.WriteLine();
		sb.WriteLine("# WebSocket listener");
		sb.WriteLine($"listener {WsPort}");
		sb.WriteLine("protocol websockets");
		sb.WriteLine("allow_anonymous true");

		if (DockerResourceConfiguration.HasCertificate)
		{
			sb.WriteLine();
			sb.WriteLine("# MQTT listener (encrypted)");
			sb.WriteLine($"listener {TlsPort}");
			sb.WriteLine("protocol mqtt");
			sb.WriteLine("allow_anonymous true");
			sb.WriteLine($"certfile {CertificateFilePath}");
			sb.WriteLine($"keyfile {CertificateKeyFilePath}");

			sb.WriteLine();
			sb.WriteLine("# WebSocket listener (encrypted)");
			sb.WriteLine($"listener {WssPort}");
			sb.WriteLine("protocol websockets");
			sb.WriteLine("allow_anonymous true");
			sb.WriteLine($"certfile {CertificateFilePath}");
			sb.WriteLine($"keyfile {CertificateKeyFilePath}");
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
			.WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("mosquitto.*running"));

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