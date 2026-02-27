namespace Testcontainers.Seq;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class SeqBuilder : ContainerBuilder<SeqBuilder, SeqContainer, SeqConfiguration>
{
    public const string SeqImage = "datalust/seq:2025.2";

    public const ushort SeqPort = 80;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeqBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public SeqBuilder()
        : this(SeqImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeqBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>datalust/seq:2025.2</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/datalust/seq" />.
    /// </remarks>
    public SeqBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeqBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/datalust/seq" />.
    /// </remarks>
    public SeqBuilder(IImage image)
        : this(new SeqConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeqBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private SeqBuilder(SeqConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override SeqConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    protected override string AcceptLicenseAgreementEnvVar { get; } = "ACCEPT_EULA";

    /// <inheritdoc />
    protected override string AcceptLicenseAgreement { get; } = "Y";

    /// <inheritdoc />
    protected override string DeclineLicenseAgreement { get; } = "N";

    /// <summary>
    /// Accepts the license agreement.
    /// </summary>
    /// <remarks>
    /// When <paramref name="acceptLicenseAgreement" /> is set to <c>true</c>, the Seq <see href="https://datalust.co/doc/eula-current.pdf">license</see> is accepted.
    /// </remarks>
    /// <param name="acceptLicenseAgreement">A boolean value indicating whether the Seq license agreement is accepted.</param>
    /// <returns>A configured instance of <see cref="SeqContainer" />.</returns>
    public override SeqBuilder WithAcceptLicenseAgreement(bool acceptLicenseAgreement)
    {
        var licenseAgreement = acceptLicenseAgreement ? AcceptLicenseAgreement : DeclineLicenseAgreement;
        return WithEnvironment(AcceptLicenseAgreementEnvVar, licenseAgreement);
    }

    /// <inheritdoc />
    public override SeqContainer Build()
    {
        Validate();
        ValidateLicenseAgreement();

        return new SeqContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override SeqBuilder Init()
    {
        return base.Init()
            .WithPortBinding(SeqPort, true)
            .WithEnvironment("SEQ_FIRSTRUN_NOAUTHENTICATION", "true")
            .WithConnectionStringProvider(new SeqConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPort(SeqPort).ForPath("/health")));
    }

    /// <inheritdoc />
    protected override SeqBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SeqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SeqBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SeqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SeqBuilder Merge(SeqConfiguration oldValue, SeqConfiguration newValue)
    {
        return new SeqBuilder(new SeqConfiguration(oldValue, newValue));
    }
}