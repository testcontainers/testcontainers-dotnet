namespace Testcontainers.Mailpit;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MailpitBuilder : ContainerBuilder<MailpitBuilder, MailpitContainer, MailpitConfiguration>
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
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>axllent/mailpit:v1.30</c>).
    /// </param>
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
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
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
    /// Sets the Mailpit <c>MP_SMTP_AUTH</c> configuration.
    /// </summary>
    /// <param name="credentials">The credentials to use for SMTP authentication.</param>
    /// <param name="allowInsecure">
    /// When <see langword="true" />, sets the Mailpit <c>MP_SMTP_AUTH_ALLOW_INSECURE</c>
    /// configuration to allow insecure PLAIN and LOGIN SMTP authentication.
    /// When <see langword="false" />, configures Mailpit to use a self-signed
    /// certificate by setting <c>MP_SMTP_TLS_CERT</c> and <c>MP_SMTP_TLS_KEY</c>
    /// to <c>sans:localhost</c>. The certificate subject and issuer are
    /// <c>CN=localhost, O=Mailpit self-signed certificate</c>.
    /// </param>
    /// <remarks>
    /// See <see href="https://mailpit.axllent.org/docs/configuration/certificates/#auto-generate-self-signed-certificates" />.
    /// </remarks>
    /// <returns>A configured <see cref="MailpitBuilder" /> instance.</returns>
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

        // TODO:
        // This prepares a full environment replacement once dictionary composition
        // supports ComposableDictionary for environments. Until that, stale keys
        // are still preserved by the current IReadOnlyDictionary merge behavior.
        var environments = DockerResourceConfiguration.Environments.ToDictionary(item => item.Key, item => item.Value);
        environments.Remove("MP_SMTP_AUTH_ALLOW_INSECURE");
        environments.Remove("MP_SMTP_TLS_CERT");
        environments.Remove("MP_SMTP_TLS_KEY");

        // https://mailpit.axllent.org/docs/configuration/smtp/#adding-smtp-authentication.
        environments["MP_SMTP_AUTH"] = $"{credentials.UserName}:{credentials.Password}";

        if (allowInsecure)
        {
            environments["MP_SMTP_AUTH_ALLOW_INSECURE"] = "1";
        }
        else
        {
            environments["MP_SMTP_TLS_CERT"] = "sans:localhost";
            environments["MP_SMTP_TLS_KEY"] = "sans:localhost";
        }

        return WithEnvironment(new OverwriteDictionary<string, string>(environments));
    }

    /// <summary>
    /// Sets the Mailpit <c>MP_MAX_MESSAGES</c> configuration.
    /// Specifies the maximum number of messages to store. Mailpit periodically
    /// deletes the oldest messages when this limit is exceeded.
    /// Set to <c>0</c> to disable automatic deletion.
    /// </summary>
    /// <param name="maxMessages">The maximum number of messages to store.</param>
    /// <returns>A configured <see cref="MailpitBuilder" /> instance.</returns>
    public MailpitBuilder WithMaxMessages(uint maxMessages)
    {
        return WithEnvironment("MP_MAX_MESSAGES", maxMessages.ToString());
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
                request.ForPath("/readyz").ForPort(WebPort)));
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