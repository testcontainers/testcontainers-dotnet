namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="DockerComposeContainer" />
internal sealed class DockerComposeRemoteContainer : DockerComposeContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeRemoteContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DockerComposeRemoteContainer(DockerComposeConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}