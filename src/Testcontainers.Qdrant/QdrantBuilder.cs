namespace Testcontainers.Qdrant;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class QdrantBuilder : ContainerBuilder<QdrantBuilder, QdrantContainer, QdrantConfiguration>
{
    public const string QdrantImage = "qdrant/qdrant:v1.8.3";

    public const ushort QdrantHttpPort = 6333;

    public const ushort QdrantGrpcPort = 6334;

    public const string QdrantTlsCertFilePath = "/qdrant/tls/cert.pem";

    public const string QdrantTlsKeyFilePath = "/qdrant/tls/key.pem";

    public QdrantBuilder() : this(new QdrantConfiguration()) =>
        DockerResourceConfiguration = Init().DockerResourceConfiguration;

    private QdrantBuilder(QdrantConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration) =>
        DockerResourceConfiguration = dockerResourceConfiguration;

    /// <summary>
    /// The API key used to secure the instance. A certificate and private key should also be
    /// provided to <see cref="WithCertificate"/> to enable Transport Layer Security (TLS).
    /// </summary>
    /// <param name="apiKey">The API key</param>
    public QdrantBuilder WithApiKey(string apiKey) =>
        Merge(DockerResourceConfiguration, new QdrantConfiguration(apiKey: apiKey))
            .WithEnvironment("QDRANT__SERVICE__API_KEY", apiKey);

    /// <summary>
    /// A certificate and private key to enable Transport Layer Security (TLS).
    /// </summary>
    /// <param name="certificate">A public certificate in PEM format</param>
    /// <param name="privateKey">A private key for the certificate in PEM format</param>
    public QdrantBuilder WithCertificate(string certificate, string privateKey)
    {
        return Merge(DockerResourceConfiguration, new QdrantConfiguration(certificate: certificate, privateKey: privateKey))
            .WithEnvironment("QDRANT__SERVICE__ENABLE_TLS", "1")
            .WithResourceMapping(Encoding.UTF8.GetBytes(certificate), QdrantTlsCertFilePath)
            .WithEnvironment("QDRANT__TLS__CERT", QdrantTlsCertFilePath)
            .WithResourceMapping(Encoding.UTF8.GetBytes(privateKey), QdrantTlsKeyFilePath)
            .WithEnvironment("QDRANT__TLS__KEY", QdrantTlsKeyFilePath);
    }

    /// <inheritdoc />
    public override QdrantContainer Build()
    {
        Validate();

        var waitStrategy = Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
        {
            var httpWaitStrategy = request.ForPort(QdrantHttpPort).ForPath("/readyz");

            // allow any certificate defined to pass validation
            if (DockerResourceConfiguration.Certificate is not null)
            {
                httpWaitStrategy.UsingTls()
                    .UsingHttpMessageHandler(new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                    });
            }

            return httpWaitStrategy;
        });

        var qdrantBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(waitStrategy);
        return new QdrantContainer(qdrantBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override QdrantBuilder Init() =>
        base.Init()
            .WithImage(QdrantImage)
            .WithPortBinding(QdrantHttpPort, true)
            .WithPortBinding(QdrantGrpcPort, true);

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