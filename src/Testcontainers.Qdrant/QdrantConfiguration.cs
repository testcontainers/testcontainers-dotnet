namespace Testcontainers.Qdrant;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class QdrantConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
    /// </summary>
    public QdrantConfiguration(string apiKey = null, string certificate = null, string privateKey = null)
    {
        ApiKey = apiKey;
        Certificate = certificate;
        PrivateKey = privateKey;
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
        PrivateKey = BuildConfiguration.Combine(oldValue.PrivateKey, newValue.PrivateKey);
    }

    /// <summary>
    /// Gets the API key used to secure Qdrant.
    /// </summary>
    public string ApiKey { get; }

    /// <summary>
    /// Gets the certificate used to configure Transport Layer Security. Certificate must be in PEM format.
    /// </summary>
    public string Certificate { get; }

    /// <summary>
    /// Gets the private key used to configure Transport Layer Security. Private key must be in PEM format.
    /// </summary>
    public string PrivateKey { get; }
}