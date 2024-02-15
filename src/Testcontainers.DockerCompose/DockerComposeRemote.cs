namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="DockerCompose" />
internal sealed class DockerComposeRemote : DockerCompose
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
    
    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken ct = default)
    {
        var command = DockerComposeCommandLineBuilder
            .FromRemoteConfiguration(RuntimeConfiguration)
            .BuildStopCommand();
            
        await ExecAsync(command, ct)
            .ConfigureAwait(false);
        await base.StopAsync(ct);
    }
}