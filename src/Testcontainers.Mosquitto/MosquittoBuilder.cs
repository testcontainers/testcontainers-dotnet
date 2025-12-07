namespace Testcontainers.Mosquitto;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MosquittoBuilder : ContainerBuilder<MosquittoBuilder, MosquittoContainer, MosquittoConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string MosquittoImage = "eclipse-mosquitto:2.0";

    public const ushort MqttPort = 1883;

    public const ushort MqttTlsPort = 8883;

    public const ushort MqttWsPort = 8080;

    public const ushort MqttWssPort = 8081;

    public const string CertificateFilePath = "/etc/mosquitto/certs/server.crt";

    public const string CertificateKeyFilePath = "/etc/mosquitto/certs/server.key";

    /// <summary>
    /// Initializes a new instance of the <see cref="MosquittoBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public MosquittoBuilder()
        : this(MosquittoImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MosquittoBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>eclipse-mosquitto:2.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/eclipse-mosquitto/tags" />.
    /// </remarks>
    public MosquittoBuilder(string image)
        : this(new MosquittoConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MosquittoBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/eclipse-mosquitto/tags" />.
    /// </remarks>
    public MosquittoBuilder(IImage image)
        : this(new MosquittoConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MosquittoBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MosquittoBuilder(MosquittoConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override MosquittoConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the public certificate and private key to enable TLS.
    /// </summary>
    /// <param name="certificate">The public certificate in PEM format.</param>
    /// <param name="certificateKey">The private key associated with the certificate in PEM format.</param>
    /// <returns>A configured instance of <see cref="MosquittoBuilder" />.</returns>
    public MosquittoBuilder WithCertificate(string certificate, string certificateKey)
    {
        return Merge(DockerResourceConfiguration, new MosquittoConfiguration(certificate: certificate, certificateKey: certificateKey))
            .WithPortBinding(MqttTlsPort, true)
            .WithPortBinding(MqttWssPort, true)
            .WithResourceMapping(Encoding.Default.GetBytes(certificate), CertificateFilePath)
            .WithResourceMapping(Encoding.Default.GetBytes(certificateKey), CertificateKeyFilePath);
    }

    /// <inheritdoc />
    public override MosquittoContainer Build()
    {
        Validate();

        const string allowAnonymous = "allow_anonymous true";

        // Maybe we should move this into the startup callback.
        var mosquittoConfig = new StringWriter();
        mosquittoConfig.NewLine = "\n";

        mosquittoConfig.WriteLine("per_listener_settings true");
        mosquittoConfig.WriteLine("log_dest stdout");
        mosquittoConfig.WriteLine("log_type information");

        mosquittoConfig.WriteLine();
        mosquittoConfig.WriteLine("persistence false");
        mosquittoConfig.WriteLine("persistence_location /mosquitto/data/");

        mosquittoConfig.WriteLine();
        mosquittoConfig.WriteLine("# MQTT, unencrypted, unauthenticated");
        mosquittoConfig.WriteLine($"listener {MqttPort} 0.0.0.0");
        mosquittoConfig.WriteLine("protocol mqtt");
        mosquittoConfig.WriteLine(allowAnonymous);

        mosquittoConfig.WriteLine();
        mosquittoConfig.WriteLine("# MQTT over WebSockets, unencrypted, unauthenticated");
        mosquittoConfig.WriteLine($"listener {MqttWsPort} 0.0.0.0");
        mosquittoConfig.WriteLine("protocol websockets");
        mosquittoConfig.WriteLine(allowAnonymous);

        if (DockerResourceConfiguration.TlsEnabled)
        {
            mosquittoConfig.WriteLine();
            mosquittoConfig.WriteLine("# MQTT, encrypted, unauthenticated");
            mosquittoConfig.WriteLine($"listener {MqttTlsPort} 0.0.0.0");
            mosquittoConfig.WriteLine("protocol mqtt");
            mosquittoConfig.WriteLine(allowAnonymous);
            mosquittoConfig.WriteLine("tls_version tlsv1.2");
            mosquittoConfig.WriteLine($"certfile {CertificateFilePath}");
            mosquittoConfig.WriteLine($"keyfile {CertificateKeyFilePath}");

            mosquittoConfig.WriteLine();
            mosquittoConfig.WriteLine("# MQTT over WebSockets, encrypted, unauthenticated");
            mosquittoConfig.WriteLine($"listener {MqttWssPort} 0.0.0.0");
            mosquittoConfig.WriteLine("protocol websockets");
            mosquittoConfig.WriteLine(allowAnonymous);
            mosquittoConfig.WriteLine("tls_version tlsv1.2");
            mosquittoConfig.WriteLine($"certfile {CertificateFilePath}");
            mosquittoConfig.WriteLine($"keyfile {CertificateKeyFilePath}");
        }

        var mosquittoBuilder = WithResourceMapping(Encoding.Default.GetBytes(mosquittoConfig.ToString()), "/mosquitto/config/mosquitto.conf");
        return new MosquittoContainer(mosquittoBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override MosquittoBuilder Init()
    {
        return base.Init()
            .WithPortBinding(MqttPort, true)
            .WithPortBinding(MqttWsPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("mosquitto.*running"));
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