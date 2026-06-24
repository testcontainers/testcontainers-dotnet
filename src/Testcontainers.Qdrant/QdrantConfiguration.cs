namespace Testcontainers.Qdrant;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class QdrantConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    /// <param name="certificate">The public certificate in PEM format.</param>
    /// <param name="certificateKey">The private key associated with the certificate in PEM format.</param>
    public QdrantConfiguration(
        string apiKey = null,
        string certificate = null,
        string certificateKey = null)
    {
        ApiKey = apiKey;
        Certificate = certificate;
        CertificateKey = certificateKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public QdrantConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public QdrantConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public QdrantConfiguration(QdrantConfiguration resourceConfiguration)
        : this(new QdrantConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
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
        CertificateKey = BuildConfiguration.Combine(oldValue.CertificateKey, newValue.CertificateKey);
    }

    /// <summary>
    /// Gets a value indicating whether TLS is enabled or not.
    /// </summary>
    public bool TlsEnabled => Certificate != null && CertificateKey != null;

    /// <summary>
    /// Gets the API key that secures the instance.
    /// </summary>
    public string ApiKey { get; }

    /// <summary>
    /// Gets the public certificate in PEM format.
    /// </summary>
    public string Certificate { get; }

    /// <summary>
    /// Gets the private key associated with the certificate in PEM format.
    /// </summary>
    public string CertificateKey { get; }
}