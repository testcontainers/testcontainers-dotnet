namespace Testcontainers.Papercut;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class PapercutContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PapercutContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public PapercutContainer(PapercutConfiguration configuration, ILogger logger)
        : base(configuration, logger)
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