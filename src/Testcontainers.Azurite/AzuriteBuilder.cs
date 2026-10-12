namespace Testcontainers.Azurite;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AzuriteBuilder : ContainerBuilder<AzuriteBuilder, AzuriteContainer, AzuriteConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string AzuriteImage = "mcr.microsoft.com/azure-storage/azurite:3.28.0";

    public const ushort BlobPort = 10000;

    public const ushort QueuePort = 10001;

    public const ushort TablePort = 10002;

    public const string AccountName = "devstoreaccount1";

    public const string AccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

    private const string CertificateFilePath = "/azurite/certs/server.crt";

    private const string CertificateKeyFilePath = "/azurite/certs/server.key";

    private static readonly ISet<AzuriteService> EnabledServices = new HashSet<AzuriteService>();

    static AzuriteBuilder()
    {
        EnabledServices.Add(AzuriteService.Blob);
        EnabledServices.Add(AzuriteService.Queue);
        EnabledServices.Add(AzuriteService.Table);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public AzuriteBuilder()
        : this(AzuriteImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>mcr.microsoft.com/azure-storage/azurite:3.28.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/microsoft/azure-storage-azurite" />.
    /// </remarks>
    public AzuriteBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/microsoft/azure-storage-azurite" />.
    /// </remarks>
    public AzuriteBuilder(IImage image)
        : this(new AzuriteConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private AzuriteBuilder(AzuriteConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override AzuriteConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Enables in-memory persistence.
    /// </summary>
    /// <remarks>
    /// By default, the in-memory is limited to 50% of the total memory on the container.
    /// </remarks>
    /// <param name="megabytes">An optional in-memory limit in megabytes.</param>
    /// <returns>A configured instance of <see cref="AzuriteBuilder" />.</returns>
    public AzuriteBuilder WithInMemoryPersistence(float? megabytes = null)
    {
        if (megabytes.HasValue)
        {
            return WithCommand("--inMemoryPersistence", "--extentMemoryLimit", megabytes.ToString());
        }
        else
        {
            return WithCommand("--inMemoryPersistence");
        }
    }

    /// <summary>
    /// Enables HTTPS for Azurite.
    /// </summary>
    /// <remarks>
    /// The Blob, Queue and Table endpoints, including the connection string,
    /// use the <c>https</c> scheme. The client must trust the server certificate.
    /// </remarks>
    /// <param name="certificateFilePath">The SSL certificate file in PEM format.</param>
    /// <param name="certificateKeyFilePath">The SSL certificate private key file in PEM format.</param>
    /// <returns>A configured instance of <see cref="AzuriteBuilder" />.</returns>
    public override AzuriteBuilder WithSsl(FilePath certificateFilePath, FilePath certificateKeyFilePath)
    {
        var sslCertificate = new SslCertificate(certificateFilePath, certificateKeyFilePath);
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(sslCertificate: sslCertificate));
    }

    /// <inheritdoc />
    public override AzuriteContainer Build()
    {
        Validate();

        var azuriteBuilder = this;

        var sslCertificate = DockerResourceConfiguration.SslCertificate;

        if (DockerResourceConfiguration.TlsEnabled)
        {
            azuriteBuilder = azuriteBuilder
                .WithResourceMapping(sslCertificate.CertificateFilePath, FilePath.Of(CertificateFilePath), fileMode: Unix.FileMode600)
                .WithResourceMapping(sslCertificate.CertificateKeyFilePath, FilePath.Of(CertificateKeyFilePath), fileMode: Unix.FileMode600)
                .WithCommand("--cert", CertificateFilePath)
                .WithCommand("--key", CertificateKeyFilePath);
        }

        var waitStrategy = Wait.ForUnixContainer();

        if (EnabledServices.Contains(AzuriteService.Blob))
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Blob service is successfully listening");
        }

        if (EnabledServices.Contains(AzuriteService.Queue))
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Queue service is successfully listening");
        }

        if (EnabledServices.Contains(AzuriteService.Table))
        {
            waitStrategy = waitStrategy.UntilMessageIsLogged("Table service is successfully listening");
        }

        // By default, the base builder waits until the container is running. However, for Azurite, a more advanced waiting strategy is necessary that requires access to the enabled services.
        // If the user does not provide a custom waiting strategy, append the default Azurite waiting strategy.
        azuriteBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? azuriteBuilder : azuriteBuilder.WithWaitStrategy(waitStrategy);
        return new AzuriteContainer(azuriteBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Init()
    {
        return base.Init()
            .WithPortBinding(BlobPort, true)
            .WithPortBinding(QueuePort, true)
            .WithPortBinding(TablePort, true)
            .WithEntrypoint("azurite")
            .WithCommand("--blobHost", "0.0.0.0", "--queueHost", "0.0.0.0", "--tableHost", "0.0.0.0")
            .WithConnectionStringProvider(new AzuriteConnectionStringProvider());
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Merge(AzuriteConfiguration oldValue, AzuriteConfiguration newValue)
    {
        return new AzuriteBuilder(new AzuriteConfiguration(oldValue, newValue));
    }
}