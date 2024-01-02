namespace Testcontainers.DockerCompose;

[PublicAPI]
public sealed class DockerComposeLocal : DockerContainer
{
    private readonly string _dockerComposeBinary =
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "docker-compose.exe" : "docker-compose";
        
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeRemote" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DockerComposeLocal(DockerComposeConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the runtime configuration.
    /// </summary>
    public DockerComposeConfiguration RuntimeConfiguration => _configuration as DockerComposeConfiguration;
    
    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken ct = default)
    {
        await Cli.Wrap(_dockerComposeBinary)
            .WithArguments(new[] {"up", "-d"})
            .WithWorkingDirectory(Path.GetDirectoryName(RuntimeConfiguration.ComposeFile)!)
            .ExecuteBufferedAsync();
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken ct = default)
    {
        await Cli.Wrap(_dockerComposeBinary)
            .WithArguments(new[] {"down"})
            .WithWorkingDirectory(Path.GetDirectoryName(RuntimeConfiguration.ComposeFile)!)
            .ExecuteBufferedAsync();
    }
}