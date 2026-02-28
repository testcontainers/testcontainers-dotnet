namespace Testcontainers.Qdrant;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class QdrantBuilder : ContainerBuilder<QdrantBuilder, QdrantContainer, QdrantConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string QdrantImage = "qdrant/qdrant:v1.13.4";

    public const ushort QdrantHttpPort = 6333;

    public const ushort QdrantGrpcPort = 6334;

    public const string CertificateFilePath = "/qdrant/tls/cert.pem";

    public const string CertificateKeyFilePath = "/qdrant/tls/key.pem";

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public QdrantBuilder()
        : this(QdrantImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>qdrant/qdrant:v1.13.4</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/qdrant/qdrant/tags" />.
    /// </remarks>
    public QdrantBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/qdrant/qdrant/tags" />.
    /// </remarks>
    public QdrantBuilder(IImage image)
        : this(new QdrantConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private QdrantBuilder(QdrantConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override QdrantConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the API key to secure the instance.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    /// <returns>A configured instance of <see cref="QdrantBuilder" />.</returns>
    public QdrantBuilder WithApiKey(string apiKey)
    {
        return Merge(DockerResourceConfiguration, new QdrantConfiguration(apiKey: apiKey))
            .WithEnvironment("QDRANT__SERVICE__API_KEY", apiKey);
    }

    /// <summary>
    /// Sets the public certificate and private key to enable TLS.
    /// </summary>
    /// <param name="certificate">The public certificate in PEM format.</param>
    /// <param name="certificateKey">The private key associated with the certificate in PEM format.</param>
    /// <returns>A configured instance of <see cref="QdrantBuilder" />.</returns>
    public QdrantBuilder WithCertificate(string certificate, string certificateKey)
    {
        return Merge(DockerResourceConfiguration, new QdrantConfiguration(certificate: certificate, certificateKey: certificateKey))
            .WithEnvironment("QDRANT__SERVICE__ENABLE_TLS", "1")
            .WithEnvironment("QDRANT__TLS__CERT", CertificateFilePath)
            .WithEnvironment("QDRANT__TLS__KEY", CertificateKeyFilePath)
            .WithResourceMapping(Encoding.Default.GetBytes(certificate), CertificateFilePath)
            .WithResourceMapping(Encoding.Default.GetBytes(certificateKey), CertificateKeyFilePath);
    }

    /// <inheritdoc />
    public override QdrantContainer Build()
    {
        Validate();

        // By default, the base builder waits until the container is running. However, for Qdrant, a more advanced waiting strategy is necessary that requires access to the configured certificate.
        // If the user does not provide a custom waiting strategy, append the default Qdrant waiting strategy.
        var qdrantBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil(DockerResourceConfiguration)));
        return new QdrantContainer(qdrantBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override QdrantBuilder Init()
    {
        return base.Init()
            .WithPortBinding(QdrantHttpPort, true)
            .WithPortBinding(QdrantGrpcPort, true);
    }

    /// <inheritdoc />
    protected override QdrantBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new QdrantConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override QdrantBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new QdrantConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override QdrantBuilder Merge(QdrantConfiguration oldValue, QdrantConfiguration newValue)
    {
        return new QdrantBuilder(new QdrantConfiguration(oldValue, newValue));
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        private readonly bool _tlsEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntil" /> class.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        public WaitUntil(QdrantConfiguration configuration)
        {
            _tlsEnabled = configuration.TlsEnabled;
        }

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            using var httpMessageHandler = new HttpClientHandler();
            httpMessageHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            var httpWaitStrategy = new HttpWaitStrategy()
                .UsingHttpMessageHandler(httpMessageHandler)
                .UsingTls(_tlsEnabled)
                .ForPort(QdrantHttpPort)
                .ForPath("/readyz");

            return await httpWaitStrategy.UntilAsync(container)
                .ConfigureAwait(false);
        }
    }
}