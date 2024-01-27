namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="DockerContainer" />
internal abstract class DockerCompose : DockerContainer
{
    private static readonly IList<string> AppLineArgs = new[] { "docker", "compose"};
    
    private static readonly IList<string> StartCommandLineArgs = new[] { "up", "-d" };
    private static readonly IList<string> StopCommandLineArgs = new[] { "down" };

    private static readonly IList<string> RemoveImagesArgs = new[] { "--rmi" };
    private static readonly IList<string> FilesArgs = new[] { "-f" };

    /// <summary>   
    /// Initializes a new instance of the <see cref="DockerCompose" /> class.
    /// </summary>
    protected DockerCompose(IContainerConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the runtime configuration.
    /// </summary>
    public DockerComposeConfiguration RuntimeConfiguration => _configuration as DockerComposeConfiguration;

    /// <summary>
    /// Builds a command line to start the docker compose
    /// </summary>
    public IList<string> BuildStartCommandLine()
    {
        return BuildConfiguration.Combine(BuildIncludeFileCommand(), StartCommandLineArgs).ToList();
    }
    
    /// <summary>
    /// Builds a command line to stop the docker compose
    /// </summary>
    public IList<string> BuildStopCommandLine()
    {
        var removeImagesArgs = RuntimeConfiguration.RemoveImages switch
        {
            RemoveImages.All => [RemoveImages.All.ToString().ToLower()],
            RemoveImages.Local => [RemoveImages.Local.ToString().ToLower()],
            _ => Array.Empty<string>(),
        };

        var stopCommand = BuildConfiguration.Combine(BuildIncludeFileCommand(), 
            StopCommandLineArgs.AsEnumerable());
        
        return removeImagesArgs.Length > 0
            ? BuildConfiguration.Combine(
                    BuildConfiguration.Combine(
                        stopCommand, RemoveImagesArgs.AsEnumerable()), 
                    removeImagesArgs)
                .ToList()
            : stopCommand.ToList();
    }

    private IEnumerable<string> BuildIncludeFileCommand()
    {
        return BuildConfiguration.Combine(
            BuildConfiguration.Combine(AppLineArgs.AsEnumerable(), FilesArgs.AsEnumerable()),
            new[] { Path.GetFileName(RuntimeConfiguration.ComposeFile) });
    }
}