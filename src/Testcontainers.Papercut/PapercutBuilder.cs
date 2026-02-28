namespace Testcontainers.Papercut;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PapercutBuilder : ContainerBuilder<PapercutBuilder, PapercutContainer, PapercutConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string PapercutImage = "changemakerstudiosus/papercut-smtp:7.0";

    public const ushort SmtpPort = 2525;

    public const ushort HttpPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="PapercutBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public PapercutBuilder()
        : this(PapercutImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PapercutBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>changemakerstudiosus/papercut-smtp:7.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/changemakerstudiosus/papercut-smtp/tags" />.
    /// </remarks>
    public PapercutBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PapercutBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/changemakerstudiosus/papercut-smtp/tags" />.
    /// </remarks>
    public PapercutBuilder(IImage image)
        : this(new PapercutConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PapercutBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private PapercutBuilder(PapercutConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override PapercutConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override PapercutContainer Build()
    {
        Validate();
        return new PapercutContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override PapercutBuilder Init()
    {
        return base.Init()
            .WithPortBinding(SmtpPort, true)
            .WithPortBinding(HttpPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/health").ForPort(HttpPort).ForResponseMessageMatching(IsInstanceHealthyAsync)));
    }

    /// <inheritdoc />
    protected override PapercutBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PapercutConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PapercutBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PapercutConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PapercutBuilder Merge(PapercutConfiguration oldValue, PapercutConfiguration newValue)
    {
        return new PapercutBuilder(new PapercutConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Determines whether the instance is healthy or not.
    /// </summary>
    /// <param name="response">The HTTP response that contains the health information.</param>
    /// <returns>A value indicating whether the instance is healthy or not.</returns>
    private static async Task<bool> IsInstanceHealthyAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        return "Papercut WebUI server started successfully.".Equals(body, StringComparison.OrdinalIgnoreCase);
    }
}