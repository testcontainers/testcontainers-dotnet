namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="DockerContainer" />
internal abstract class DockerCompose : DockerContainer
{
    /// <summary>   
    /// Initializes a new instance of the <see cref="DockerCompose" /> class.
    /// </summary>
    protected DockerCompose(IContainerConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
        RuntimeConfiguration = _configuration as DockerComposeConfiguration;
    }

    /// <summary>
    /// Gets the runtime configuration.
    /// </summary>
    protected DockerComposeConfiguration RuntimeConfiguration { get; }
}