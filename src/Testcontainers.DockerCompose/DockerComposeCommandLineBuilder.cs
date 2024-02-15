namespace Testcontainers.DockerCompose;

/// <summary>
/// Instance of the Docker Compose command line builder
/// </summary>
internal sealed class DockerComposeCommandLineBuilder
{
    public const string DockerComposeFileName = "/docker-compose.yaml";
    public const string DockerAppName = "docker";

    private const string DockerComposeArgs = "compose";
    private const string StartCommandArgs = "up";
    private const string DetachCommandArgs = "-d";
    private const string FileCommandArgs = "-f";
    private const string StopCommandLineArgs = "down";
    private static readonly IList<string> RemoveImagesArgs = new[] { "--rmi" };
    
    private readonly DockerComposeConfiguration _configuration;
    private readonly bool _local;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeCommandLineBuilder" /> class. 
    /// </summary>
    /// <param name="configuration">The Docker resource configuration.</param>
    /// <param name="local">Indicates whether local compose is enabled.</param>
    private DockerComposeCommandLineBuilder(DockerComposeConfiguration configuration, bool local)
    {
        _configuration = configuration;
        _local = local;
    }
    
    /// <summary>
    /// Builds a command line for remote compose from the <see cref="DockerComposeCommandLineBuilder" />configuration. 
    /// </summary>
    /// <param name="configuration">The Docker resource configuration.</param>
    /// <returns>A configured instance of <see cref="DockerComposeCommandLineBuilder" />.</returns>
    public static DockerComposeCommandLineBuilder FromRemoteConfiguration(DockerComposeConfiguration configuration)
    {
        return new DockerComposeCommandLineBuilder(configuration, false);
    }

    /// <summary>
    /// Builds a command line for local compose from the <see cref="DockerComposeCommandLineBuilder" />configuration.
    /// </summary>
    /// <param name="configuration">The Docker resource configuration.</param>
    /// <returns>A configured instance of <see cref="DockerComposeCommandLineBuilder" />.</returns>
    public static DockerComposeCommandLineBuilder FromLocalConfiguration(DockerComposeConfiguration configuration)
    {
        return new DockerComposeCommandLineBuilder(configuration, true);
    }

    /// <summary>
    /// Builds a command line to start the docker compose
    /// </summary>
    public IList<string> BuildStartCommand()
    {
        var stopCommand =   _local ? new[] { DockerComposeArgs, FileCommandArgs, 
                Path.GetFileName(_configuration.ComposeFile), StartCommandArgs, DetachCommandArgs }
           : new [] { DockerComposeArgs, StartCommandArgs };

        if (!_local)
        {
            return stopCommand;
        }
        
        if (_configuration.Options.Any())
        {
            stopCommand = stopCommand.Concat(_configuration.Options).ToArray();
        }

        return stopCommand;
    }
    
    /// <summary>
    /// Builds a command line to stop the docker compose
    /// </summary>
    public IList<string> BuildStopCommand()
    {
        var removeImagesArgs = _configuration.RemoveImages switch
        {
            RemoveImages.All => [RemoveImages.All.ToString().ToLower()],
            RemoveImages.Local => [RemoveImages.Local.ToString().ToLower()],
            _ => Array.Empty<string>(),
        };
        
        var stopCommand = _local
            ? new[] { DockerComposeArgs, StopCommandLineArgs }
            : new[] { DockerAppName, DockerComposeArgs, FileCommandArgs, 
                Path.GetFileName(_configuration.ComposeFile), StopCommandLineArgs };
        
        return removeImagesArgs.Length > 0
            ? stopCommand.Concat(RemoveImagesArgs).ToList()
            : stopCommand.ToList();
    }
}