namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="DockerContainer" />
internal abstract class DockerCompose : DockerContainer
{
    private static readonly IList<string> StartCommandLineArgs = new[] { "docker", "compose", "up", "-d" };
    private static readonly IList<string> StopCommandLineArgs = new[] { "docker", "compose", "down" };

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
    /// Gets the command line to start the docker compose
    /// </summary>
    public IList<string> StartCommandLine => BuildConfiguration.Combine(StartCommandLineArgs, 
        RuntimeConfiguration.Options).ToList();

    /// <summary>
    /// Gets the command line to stop the docker compose
    /// </summary>
    public IList<string> StopCommandLine => StopCommandLineArgs;
}