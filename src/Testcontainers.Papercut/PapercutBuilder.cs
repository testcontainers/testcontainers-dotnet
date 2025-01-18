namespace Testcontainers.Papercut;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PapercutBuilder : ContainerBuilder<PapercutBuilder, PapercutContainer, PapercutConfiguration>
{
    public const string PapercutImage = "changemakerstudiosus/papercut-smtp:latest";

    public const ushort SmtpPort = 25;

    public const ushort HttpPort = 80;

    /// <summary>
    /// Initializes a new instance of the <see cref="PapercutBuilder" /> class.
    /// </summary>
    public PapercutBuilder()
        : this(new PapercutConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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
            .WithImage(PapercutImage)
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