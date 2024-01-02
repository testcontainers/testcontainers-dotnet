namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public class DockerComposeRemote : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeRemote" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DockerComposeRemote(DockerComposeConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the runtime configuration.
    /// </summary>
    public DockerComposeConfiguration RuntimeConfiguration => _configuration as DockerComposeConfiguration;
    
    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken ct = default)
    {
        await ExecAsync(new[] { "docker", "compose", "down"}, ct)
            .ConfigureAwait(false);
        await base.StopAsync(ct);
    }
}