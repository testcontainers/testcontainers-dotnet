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
        var builder = DockerComposeCommandLineBuilder.FromLocalConfiguration(RuntimeConfiguration);
        
        await Cli.Wrap(_dockerBinary)
            .WithArguments(builder.BuildStartCommand())
            .WithWorkingDirectory(Path.GetDirectoryName(RuntimeConfiguration.ComposeFile)!)
            .ExecuteBufferedAsync()
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken ct = default)
    {
        var builder = DockerComposeCommandLineBuilder.FromLocalConfiguration(RuntimeConfiguration);
        
        await Cli.Wrap(_dockerBinary)
            .WithArguments(builder.BuildStopCommand())
            .WithWorkingDirectory(Path.GetDirectoryName(RuntimeConfiguration.ComposeFile)!)
            .ExecuteBufferedAsync()
            .ConfigureAwait(false);
    }
}