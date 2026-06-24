namespace Testcontainers.Mosquitto;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MosquittoConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MosquittoConfiguration" /> class.
    /// </summary>
    /// <param name="certificate">The public certificate in PEM format.</param>
    /// <param name="certificateKey">The private key associated with the certificate in PEM format.</param>
    public MosquittoConfiguration(
        string certificate = null,
        string certificateKey = null)
    {
        Certificate = certificate;
        CertificateKey = certificateKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MosquittoConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MosquittoConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MosquittoConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MosquittoConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MosquittoConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MosquittoConfiguration(MosquittoConfiguration resourceConfiguration)
        : this(new MosquittoConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MosquittoConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MosquittoConfiguration(MosquittoConfiguration oldValue, MosquittoConfiguration newValue)
        : base(oldValue, newValue)
    {
        Certificate = BuildConfiguration.Combine(oldValue.Certificate, newValue.Certificate);
        CertificateKey = BuildConfiguration.Combine(oldValue.CertificateKey, newValue.CertificateKey);
    }

    /// <summary>
    /// Gets a value indicating whether TLS is enabled or not.
    /// </summary>
    public bool TlsEnabled => Certificate != null && CertificateKey != null;

    /// <summary>
    /// Gets the public certificate in PEM format.
    /// </summary>
    public string Certificate { get; }

    /// <summary>
    /// Gets the private key associated with the certificate in PEM format.
    /// </summary>
    public string CertificateKey { get; }
}