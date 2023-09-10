using System.IO;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Testcontainers.Qdrant;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class QdrantBuilder : ContainerBuilder<QdrantBuilder, QdrantContainer, QdrantConfiguration>
{
	public const string QdrantImage = "qdrant/qdrant:v1.5.0";

	public const ushort QdrantHttpPort = 6333;

	public const ushort QdrantGrpcPort = 6334;

	public const string QdrantLocalConfigurationFilePath = "/qdrant/config/local.yaml";
	
	public const string QdrantTlsCertFilePath = "/qdrant/tls/cert.pem";
	
	public const string QdrantTlsKeyFilePath = "/qdrant/tls/key.pem";

	public QdrantBuilder() : this(new QdrantConfiguration()) =>
		DockerResourceConfiguration = Init().DockerResourceConfiguration;

	private QdrantBuilder(QdrantConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration) =>
		DockerResourceConfiguration = dockerResourceConfiguration;

	/// <summary>
	/// A path to a configuration file with which configure the instance.
	/// </summary>
	/// <param name="configurationFilePath">The path to the configuration file</param>
	public QdrantBuilder WithConfigFile(string configurationFilePath) =>
		Merge(DockerResourceConfiguration, new QdrantConfiguration(configurationFilePath: configurationFilePath))
			.WithBindMount(configurationFilePath, QdrantLocalConfigurationFilePath);
    
	/// <summary>
	/// The API key used to secure the instance. A certificate should also be provided to <see cref="WithCertificate"/>
	/// to enable TLS
	/// </summary>
	/// <param name="apiKey">The API key</param>
	public QdrantBuilder WithApiKey(string apiKey) =>
		Merge(DockerResourceConfiguration, new QdrantConfiguration(apiKey: apiKey))
			.WithEnvironment("QDRANT__SERVICE__API_KEY", apiKey);
	
	/// <summary>
	/// A certificate to use to enable Transport Layer Security (TLS). The certificate must contain the
	/// private key.
	/// </summary>
	/// <param name="certificate">A certificate containing a private key</param>
	public QdrantBuilder WithCertificate(X509Certificate2 certificate)
	{
		if (!certificate.HasPrivateKey)
		{
			throw new ArgumentException("certificate must contain a private key", nameof(certificate));
		}
		
		var builder = new StringBuilder();
		builder.AppendLine("-----BEGIN CERTIFICATE-----");
		builder.AppendLine(Convert.ToBase64String(certificate.RawData, Base64FormattingOptions.InsertLineBreaks));
		builder.AppendLine("-----END CERTIFICATE-----");
		var cert = builder.ToString();
		builder.Clear();
		
		var keyPair = DotNetUtilities.GetKeyPair(certificate.PrivateKey);
		var pemWriter = new PemWriter(new StringWriter(builder));
		pemWriter.WriteObject(keyPair.Private);
		var key = builder.ToString();
        
		return Merge(DockerResourceConfiguration, new QdrantConfiguration(certificate: certificate))
			.WithEnvironment("QDRANT__SERVICE__ENABLE_TLS", "1")
			.WithResourceMapping(Encoding.UTF8.GetBytes(cert), QdrantTlsCertFilePath)
			.WithEnvironment("QDRANT__TLS__CERT", QdrantTlsCertFilePath)
			.WithResourceMapping(Encoding.UTF8.GetBytes(key), QdrantTlsKeyFilePath)
			.WithEnvironment("QDRANT__TLS__KEY", QdrantTlsKeyFilePath);
	}
    
	/// <inheritdoc />
	public override QdrantContainer Build()
	{
		Validate();
		return new QdrantContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
	}

	/// <inheritdoc />
	protected override QdrantBuilder Init() =>
		base.Init()
			.WithImage(QdrantImage)
			.WithPortBinding(QdrantHttpPort, true)
			.WithPortBinding(QdrantGrpcPort, true)
			.WithWaitStrategy(Wait.ForUnixContainer()
				.UntilMessageIsLogged(".*Actix runtime found; starting in Actix runtime.*"));

	/// <inheritdoc />
	protected override QdrantBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration) =>
		Merge(DockerResourceConfiguration, new QdrantConfiguration(resourceConfiguration));

	/// <inheritdoc />
	protected override QdrantBuilder Merge(QdrantConfiguration oldValue, QdrantConfiguration newValue) =>
		new(new QdrantConfiguration(oldValue, newValue));

	/// <inheritdoc />
	protected override QdrantConfiguration DockerResourceConfiguration { get; }

	/// <inheritdoc />
	protected override QdrantBuilder Clone(IContainerConfiguration resourceConfiguration) =>
		Merge(DockerResourceConfiguration, new QdrantConfiguration(resourceConfiguration));
}
