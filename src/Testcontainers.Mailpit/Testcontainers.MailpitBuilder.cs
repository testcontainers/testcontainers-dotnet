using System.Net;

namespace Testcontainers.Mailpit;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MailpitBuilder
    : ContainerBuilder<MailpitBuilder, MailpitContainer, MailpitConfiguration>
{
    public const string MAILPIT_IMAGE = "axllent/mailpit";
    public const ushort MAILPIT_SMTP_PORT = 1025;
    public const ushort MAILPIT_API_PORT = 8025;

    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitBuilder" /> class.
    /// </summary>
    public MailpitBuilder()
        : this(new MailpitConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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
    /// Sets the Mailpit MP_SMTP_AUTH config. Also sets MP_SMTP_AUTH_ALLOW_INSECURE to true.
    /// </summary>
    /// <param name="authCredentials">List of credentials to be used in SMTP authentication</param>
    /// <returns>A configured instance of <see cref="Testcontainers.MailpitBuilder" />.</returns>
    public MailpitBuilder WithSmtpAuthCredentials(
        List<MailpitConfiguration.AuthCredentials> authCredentials
    )
    {
        return Merge(
                DockerResourceConfiguration,
                new MailpitConfiguration(
                    smtpAuthCredentials: authCredentials,
                    smtpAuthAllowInsecure: true
                )
            )
            .WithEnvironment(
                "MP_SMTP_AUTH",
                string.Join(" ", authCredentials.Select(e => $"{e.Username}:{e.Password}"))
            )
            .WithEnvironment("MP_SMTP_AUTH_ALLOW_INSECURE", "1");
    }

    /// <summary>
    /// Sets the Mailpit MP_SMTP_AUTH_ALLOW_INSECURE config.
    /// Typically STARTTLS is enforced for all SMTP authentication. This option allows insecure PLAIN & LOGIN SMTP authentication.
    /// </summary>
    /// <param name="allowInsecure">Whether or not to allow PLAIN & LOGIN SMTP authentication</param>
    /// <returns>A configured instance of <see cref="Testcontainers.MailpitBuilder" />.</returns>
    public MailpitBuilder WithSmtpAuthAllowInsecure(bool allowInsecure)
    {
        return Merge(
                DockerResourceConfiguration,
                new MailpitConfiguration(smtpAuthAllowInsecure: allowInsecure)
            )
            .WithEnvironment("MP_SMTP_AUTH_ALLOW_INSECURE", allowInsecure ? "1" : "0");
    }

    /// <summary>
    /// Sets the Mailpit MP_MAX_MESSAGES config.
    /// Maximum number of messages to store. Mailpit will periodically delete the oldest messages if greater than this. Set to 0 to disable auto-deletion.
    /// </summary>
    /// <param name="maxMessages">Maximum number to set</param>
    /// <returns>A configured instance of <see cref="Testcontainers.MailpitBuilder" />.</returns>
    public MailpitBuilder WithMaxMessages(uint maxMessages)
    {
        return Merge(
                DockerResourceConfiguration,
                new MailpitConfiguration(maxMessages: maxMessages)
            )
            .WithEnvironment("MP_MAX_MESSAGES", maxMessages.ToString());
    }

    /// <inheritdoc />
    public override MailpitContainer Build()
    {
        Validate();
        return new MailpitContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override MailpitBuilder Init()
    {
        return base.Init()
            .WithImage(MAILPIT_IMAGE)
            .WithPortBinding(MAILPIT_SMTP_PORT, true)
            .WithPortBinding(MAILPIT_API_PORT, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(r =>
                        r.ForPort(MAILPIT_API_PORT)
                            .ForPath("/api/v1/info")
                            .ForStatusCode(HttpStatusCode.OK)
                    )
            );
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard
            .Argument(
                DockerResourceConfiguration.SmtpAuthCredentials,
                nameof(DockerResourceConfiguration.SmtpAuthCredentials)
            )
            .NotNull();
    }

    /// <inheritdoc />
    protected override MailpitBuilder Clone(
        IResourceConfiguration<CreateContainerParameters> resourceConfiguration
    )
    {
        return Merge(DockerResourceConfiguration, new MailpitConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MailpitBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MailpitConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MailpitBuilder Merge(
        MailpitConfiguration oldValue,
        MailpitConfiguration newValue
    )
    {
        return new MailpitBuilder(new MailpitConfiguration(oldValue, newValue));
    }
}
