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
    /// Gets the SMTP port.
    /// </summary>
    public ushort SmtpPort => GetMappedPublicPort(MailpitBuilder.SmtpPort);

    /// <summary>
    /// Gets the Mailpit web server address.
    /// </summary>
    /// <remarks>
    /// This address can also be used as the base URL for the <see href="https://mailpit.axllent.org/docs/api-v1/">REST API</see>.
    /// </remarks>
    /// <returns>The Mailpit web server address.</returns>
    public string GetWebAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(MailpitBuilder.WebPort)).ToString();
    }
}