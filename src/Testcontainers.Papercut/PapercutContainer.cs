namespace Testcontainers.Papercut
{
    /// <inheritdoc cref="DockerContainer" />
    [PublicAPI]
    public sealed class PapercutContainer : DockerContainer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PapercutContainer" /> class.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        /// <param name="logger">The logger.</param>
        public PapercutContainer(PapercutConfiguration configuration, ILogger logger)
            : base(configuration, logger)
        {
        }

        public string WebUrl => "http://" + Hostname + ":" + GetMappedPublicPort(PapercutBuilder.WebInterfacePort);
        public int PublicSmtpPort => GetMappedPublicPort(PapercutBuilder.SmtpPort);
    }
}