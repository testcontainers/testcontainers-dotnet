namespace Testcontainers.Mailpit;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MailpitBuilder
    : ContainerBuilder<MailpitBuilder, MailpitContainer, MailpitConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string MailpitImage = "axllent/mailpit:v1.30";

    public const ushort SmtpPort = 1025;

    public const ushort WebPort = 8025;

    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public MailpitBuilder()
        :this(MailpitImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitBuilder" /> class.
    /// </summary>
    /// <param name="image">The full Docker image name, including the image repository and tag (e.g., <c>axllent/mailpit:v1.30</c>).</param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/axllent/mailpit/tags" />.
    /// </remarks>
    public MailpitBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitBuilder" /> class.
    /// </summary>
    /// <param name="image">An <see cref="IImage" /> instance that specifies the Docker image to be used for the container builder configuration.</param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/axllent/mailpit/tags" />.
    /// </remarks>
    public MailpitBuilder(IImage image)
        : this(new MailpitConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private MailpitBuilder(MailpitConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override MailpitConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Mailpit MP_SMTP_AUTH config.
    /// </summary>
    /// <param name="credentials">The credentials to be used in SMTP authentication.</param>
    /// <param name="allowInsecure">
    /// When <see langword="true"/>, the MP_SMTP_AUTH_ALLOW_INSECURE config is set to true to allow insecure PLAIN and LOGIN SMTP authentication.
    /// When <see langword="false"/>, a self-signed certificate is used. Its subject and issuer are <c>CN=localhost, O=Mailpit self-signed certificate</c>.
    /// </param>
    /// <returns>A configured instance of <see cref="MailpitBuilder" />.</returns>
    public MailpitBuilder WithSmtpAuthCredentials(NetworkCredential credentials, bool allowInsecure)
    {
        if (credentials == null)
        {
            throw new ArgumentNullException(nameof(credentials));
        }

        if (credentials.UserName.Contains(":"))
        {
            throw new ArgumentException("The UserName cannot contain a colon (:) character.", nameof(credentials));
        }

        // https://mailpit.axllent.org/docs/configuration/smtp/#adding-smtp-authentication
        var builder = Merge(DockerResourceConfiguration, new MailpitConfiguration(smtpAuthCredentials: credentials, smtpAuthAllowInsecure: allowInsecure))
            .WithEnvironment("MP_SMTP_AUTH", $"{credentials.UserName}:{credentials.Password}");

        return allowInsecure
            ? builder
                .WithEnvironment("MP_SMTP_AUTH_ALLOW_INSECURE", "1")
            : builder
                 // https://mailpit.axllent.org/docs/configuration/certificates/#auto-generate-self-signed-certificates
                .WithEnvironment("MP_SMTP_TLS_CERT", "sans:localhost")
                .WithEnvironment("MP_SMTP_TLS_KEY", "sans:localhost");
    }

    /// <summary>
    /// Sets the Mailpit MP_MAX_MESSAGES config.
    /// Maximum number of messages to store. Mailpit will periodically delete the oldest messages if greater than this. Set to 0 to disable auto-deletion.
    /// </summary>
    /// <param name="maxMessages">The maximum number of messages to set.</param>
    /// <returns>A configured instance of <see cref="MailpitBuilder" />.</returns>
    public MailpitBuilder WithMaxMessages(uint maxMessages)
    {
        return Merge(DockerResourceConfiguration, new MailpitConfiguration(maxMessages: maxMessages))
            .WithEnvironment("MP_MAX_MESSAGES", maxMessages.ToString());
    }

    /// <inheritdoc />
    public override MailpitContainer Build()
    {
        Validate();
        return new MailpitContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override MailpitBuilder Init()
    {
        return base.Init()
            .WithPortBinding(SmtpPort, true)
            .WithPortBinding(WebPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                // https://mailpit.axllent.org/docs/integration/healthcheck/
                request.ForPort(WebPort).ForPath("/readyz")));
    }

    /// <inheritdoc />
    protected override MailpitBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MailpitConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MailpitBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MailpitConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MailpitBuilder Merge(MailpitConfiguration oldValue, MailpitConfiguration newValue)
    {
        return new MailpitBuilder(new MailpitConfiguration(oldValue, newValue));
    }
}
