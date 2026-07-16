namespace Testcontainers.Mailpit;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MailpitContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MailpitContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MailpitContainer(MailpitConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// The SMTP server port.
    /// </summary>
    public ushort SmtpPort => GetMappedPublicPort(MailpitBuilder.SmtpPort);

    /// <summary>
    /// Gets the web server address of the user interface. Can also be used as the base URL for the <see href="https://mailpit.axllent.org/docs/api-v1/"> REST API</see>.
    /// </summary>
    public string GetWebAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(MailpitBuilder.WebPort)).ToString();
    }
}
