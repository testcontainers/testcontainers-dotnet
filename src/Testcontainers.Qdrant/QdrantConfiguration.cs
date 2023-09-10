using System.Security.Cryptography.X509Certificates;

namespace Testcontainers.Qdrant;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class QdrantConfiguration : ContainerConfiguration
{
	/// <summary>
	/// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
	/// </summary>
	public QdrantConfiguration(string apiKey = null, X509Certificate2 certificate = null, string configurationFilePath = null)
	{
		ApiKey = apiKey;
		Certificate = certificate;
		ConfigurationFilePath = configurationFilePath;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
	/// </summary>
	/// <param name="resourceConfiguration">The Docker resource configuration.</param>
	public QdrantConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
		: base(resourceConfiguration)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
	/// </summary>
	/// <param name="resourceConfiguration">The Docker resource configuration.</param>
	public QdrantConfiguration(IContainerConfiguration resourceConfiguration)
		: base(resourceConfiguration)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
	/// </summary>
	/// <param name="resourceConfiguration">The Docker resource configuration.</param>
	public QdrantConfiguration(QdrantConfiguration resourceConfiguration)
		: this(new QdrantConfiguration(), resourceConfiguration)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
	/// </summary>
	/// <param name="oldValue">The old Docker resource configuration.</param>
	/// <param name="newValue">The new Docker resource configuration.</param>
	public QdrantConfiguration(QdrantConfiguration oldValue, QdrantConfiguration newValue)
		: base(oldValue, newValue)
	{
		ApiKey = BuildConfiguration.Combine(oldValue.ApiKey, newValue.ApiKey);
		Certificate = BuildConfiguration.Combine(oldValue.Certificate, newValue.Certificate);
		ConfigurationFilePath = BuildConfiguration.Combine(oldValue.ConfigurationFilePath, newValue.ConfigurationFilePath);
	}
	
	/// <summary>
	/// Gets the API key used to secure Qdrant
	/// </summary>
	public string ApiKey { get; }
	
	/// <summary>
	/// Gets the certificate used to configure Transport Layer Security
	/// </summary>
	public X509Certificate2 Certificate { get; }

	/// <summary>
	/// Gets the path to the configuration file used to configure Qdrant
	/// </summary>
	public string ConfigurationFilePath { get; }
}
