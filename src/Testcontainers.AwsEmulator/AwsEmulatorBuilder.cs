namespace Testcontainers.AwsEmulator;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AwsEmulatorBuilder : ContainerBuilder<AwsEmulatorBuilder, AwsEmulatorContainer, AwsEmulatorConfiguration>
{
    public const ushort AwsEmulatorPort = 9297;

    public const string Region = "us-east-1";

    public const string HomeDirectoryPath = "/tmp/testcontainers-aws";

    // The emulator writes the generated files to a
    // directory named after the port it listens on.
    public const string CertificateFilePath = HomeDirectoryPath + "/.aws/emulator/9297/ca.pem";

    public const string CredentialsFilePath = HomeDirectoryPath + "/.aws/emulator/9297/emulator_credentials";

    /// <summary>
    /// Initializes a new instance of the <see cref="AwsEmulatorBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>ghcr.io/xsavikx/aws-emulator:1.0.509.1-20261118.43</c>).
    /// </param>
    /// <remarks>
    /// AWS does not publish an official AWS Emulator image yet. Docker image tags
    /// of an unofficial community image available at <see href="https://github.com/xSAVIKx/aws-emulator-docker/pkgs/container/aws-emulator" />.
    /// The image must use the <c>aws-emulator</c> executable as its entrypoint.
    /// <include file="AwsEmulatorBuilder.Generated.xml" path="doc/member[@name='ctor']/*" />
    /// </remarks>
    public AwsEmulatorBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwsEmulatorBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// AWS does not publish an official AWS Emulator image yet. Docker image tags
    /// of an unofficial community image available at <see href="https://github.com/xSAVIKx/aws-emulator-docker/pkgs/container/aws-emulator" />.
    /// The image must use the <c>aws-emulator</c> executable as its entrypoint.
    /// <include file="AwsEmulatorBuilder.Generated.xml" path="doc/member[@name='ctor']/*" />
    /// </remarks>
    public AwsEmulatorBuilder(IImage image)
        : this(new AwsEmulatorConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwsEmulatorBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private AwsEmulatorBuilder(AwsEmulatorConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override AwsEmulatorConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override AwsEmulatorContainer Build()
    {
        Validate();
        return new AwsEmulatorContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override AwsEmulatorBuilder Init()
    {
        // The module reads the generated certificate and credentials from the home
        // directory. Set it instead of relying on the image's home directory.
        return base.Init()
            .WithPortBinding(AwsEmulatorPort, true)
            .WithCommand("--bind", "0.0.0.0", "--port", "9297", "--auth", "on")
            .WithEnvironment("HOME", HomeDirectoryPath)
            .WithEnvironment("EMULATOR_TELEMETRY", "off")
            .WithConnectionStringProvider(() => new AwsEmulatorConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()));
    }

    /// <inheritdoc />
    protected override AwsEmulatorBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AwsEmulatorConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AwsEmulatorBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AwsEmulatorConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AwsEmulatorBuilder Merge(AwsEmulatorConfiguration oldValue, AwsEmulatorConfiguration newValue)
    {
        return new AwsEmulatorBuilder(new AwsEmulatorConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Waits until the HTTPS request <c>GET /__admin/health</c>
    /// to port <c>9297</c> is completed successfully.
    /// </summary>
    /// <remarks>
    /// The emulator generates its certificate during the startup. The wait
    /// strategy does not validate it, because the module reads the certificate
    /// only after the emulator is ready.
    /// </remarks>
    private sealed class WaitUntil : IWaitUntil
    {
        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            using var httpMessageHandler = new HttpClientHandler();
            httpMessageHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            var httpWaitStrategy = new HttpWaitStrategy()
                .UsingHttpMessageHandler(httpMessageHandler)
                .UsingTls()
                .ForPort(AwsEmulatorPort)
                .ForPath("/__admin/health");

            return await httpWaitStrategy.UntilAsync(container)
                .ConfigureAwait(false);
        }
    }
}