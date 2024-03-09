namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public abstract class DockerComposeContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    protected DockerComposeContainer(DockerComposeConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}