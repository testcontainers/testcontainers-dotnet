namespace Testcontainers.Papercut;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class PapercutContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PapercutContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public PapercutContainer(PapercutConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the SMTP port.
    /// </summary>
    public ushort SmtpPort => GetMappedPublicPort(PapercutBuilder.SmtpPort);

    /// <summary>
    /// Gets the Papercut base address.
    /// </summary>
    /// <returns>The Papercut base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(PapercutBuilder.HttpPort)).ToString();
    }
}