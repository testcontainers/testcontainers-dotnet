namespace Testcontainers.Mailpit;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MailpitContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public MailpitContainer(MailpitConfiguration configuration, ILogger logger)
        : base(configuration, logger) { }

    /// <summary>
    /// SMTP server port.
    /// </summary>
    public ushort SmtpPort
    {
        get => GetMappedPublicPort(MailpitBuilder.MAILPIT_SMTP_PORT);
    }

    /// <summary>
    /// Web API server port.
    /// </summary>
    public ushort ApiPort
    {
        get => GetMappedPublicPort(MailpitBuilder.MAILPIT_API_PORT);
    }
}
