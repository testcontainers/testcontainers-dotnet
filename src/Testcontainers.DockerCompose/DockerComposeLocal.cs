namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="DockerCompose" />
internal sealed class DockerComposeLocal : DockerCompose
{
    private readonly string _dockerBinary =
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "docker.exe" : "docker";
        
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeRemote" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DockerComposeLocal(DockerComposeConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
    }
    
    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken ct = default)
    {
        await Cli.Wrap(_dockerBinary)
            .WithArguments(StartCommandLine.Skip(1))
            .WithWorkingDirectory(Path.GetDirectoryName(RuntimeConfiguration.ComposeFile)!)
            .ExecuteBufferedAsync();
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken ct = default)
    {
        await Cli.Wrap(_dockerBinary)
            .WithArguments(StopCommandLine.Skip(1))
            .WithWorkingDirectory(Path.GetDirectoryName(RuntimeConfiguration.ComposeFile)!)
            .ExecuteBufferedAsync();
    }
}